using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Invoice;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

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
        public CancelInvoiceCommandHandler(IRepositoryAsync<Invoice> invoiceRepository, IMapper mapper)
        {
            _invoiceRepository = invoiceRepository;
            _mapper = mapper;
        }
        public async Task<Response<InvoiceDto>> Handle(CancelInvoiceCommand request, CancellationToken cancellationToken)
        {
            var invoiceExist = await _invoiceRepository.GetByIdAsync(request.Id);
            if (invoiceExist == null)
            {
                throw new ApiException($"No existe una factura con el Id {request.CustomerId}");
            }
            invoiceExist.StatusId = 17;
            await _invoiceRepository.UpdateAsync(invoiceExist);
            await _invoiceRepository.SaveChangesAsync();
            request.StatusId = 17;
            var invoiceDto = _mapper.Map<InvoiceDto>(request);
            return new Response<InvoiceDto>(invoiceDto, "Factura Cancelada Existosamente");
        }
    }
}
