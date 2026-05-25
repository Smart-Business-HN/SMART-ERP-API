using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Invoice;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.AccountingPostingService;
using SMART.ERP.Application.Services.InventoryMovementService;
using SMART.ERP.Application.Services.JwtService;
using SMART.ERP.Application.Services.ProductCacheInvalidator;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Domain.Enums;

namespace SMART.ERP.Application.Features.InvoiceFeature.Commands.CancelInvoiceCommand
{
    public class CancelInvoiceCommand : IRequest<Response<InvoiceDto>>
    {
        public int Id { get; set; }
        public Guid CustomerId { get; set; }
        public int BranchOfficeId { get; set; }
        public Guid? UserId { get; set; }
        public DateTime CreationDate { get; set; }
        public string? Observations { get; set; }
        public string? TermsAndConditions { get; set; }
        public List<ProductSoldDto>? ProductsSold { get; set; }
        public int StatusId { get; set; }
        public string? PurchaseOrderCode { get; set; }
        public string? SagCode { get; set; }
        public string? ExemptOrderNumber { get; set; }
        public string? ExemptedRegistrationCertificateNumber { get; set; }
    }
    public class CancelInvoiceCommandHandler : IRequestHandler<CancelInvoiceCommand, Response<InvoiceDto>>
    {
        private readonly IRepositoryAsync<Invoice> _invoiceRepository;
        private readonly IMapper _mapper;
        private readonly IJwtService _jwtService;
        private readonly IInventoryMovementService _movementService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IProductCacheInvalidator _cacheInvalidator;
        private readonly IAccountingPostingService _accountingPostingService;
        public CancelInvoiceCommandHandler(IRepositoryAsync<Invoice> invoiceRepository, IMapper mapper, IJwtService jwtService, IInventoryMovementService movementService, IUnitOfWork unitOfWork, IProductCacheInvalidator cacheInvalidator, IAccountingPostingService accountingPostingService)
        {
            _invoiceRepository = invoiceRepository;
            _mapper = mapper;
            _jwtService = jwtService;
            _movementService = movementService;
            _unitOfWork = unitOfWork;
            _cacheInvalidator = cacheInvalidator;
            _accountingPostingService = accountingPostingService;
        }
        public async Task<Response<InvoiceDto>> Handle(CancelInvoiceCommand request, CancellationToken cancellationToken)
        {
            var invoiceExist = await _invoiceRepository.GetByIdAsync(request.Id);
            if (invoiceExist == null)
            {
                throw new ApiException($"No existe una factura con el Id {request.Id}");
            }

            var userId = _jwtService.GetUidToken();
            var userName = _jwtService.GetSubjectToken();

            await _unitOfWork.ExecuteInTransactionAsync(async ct =>
            {
                invoiceExist.StatusId = 17;
                invoiceExist.ModificationDate = DateTime.UtcNow;
                invoiceExist.ModificatedBy = userName;
                await _invoiceRepository.UpdateAsync(invoiceExist);
                await _invoiceRepository.SaveChangesAsync();

                // Revierte en el Kardex los movimientos de venta de esta factura (devuelve el stock).
                // Solo afecta a facturas creadas tras la integración con el Kardex; las antiguas no
                // tienen movimientos que revertir.
                await _movementService.RecordCancellationForDocumentAsync(
                    "Invoice", invoiceExist.Id, DateTime.UtcNow,
                    KardexMovementType.InvoiceCancellation, userId, userName, ct);

                await _accountingPostingService.ReverseDocumentPostingAsync("Invoice", invoiceExist.Id, ct);
            }, cancellationToken);

            await _cacheInvalidator.InvalidateAsync(cancellationToken);

            request.StatusId = 17;
            var invoiceDto = _mapper.Map<InvoiceDto>(request);
            return new Response<InvoiceDto>(invoiceDto, "Factura Cancelada Existosamente");
        }
    }
}
