using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.PurchaseBill;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.ProviderSpecification;
using SMART.ERP.Application.Specifications.PurchaseOrderSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.PurchaseBillFeature.Commands.CreatePurchaseBillCommand
{
    public class CreatePurchaseBillCommand : IRequest<Response<PurchaseBillDto>>
    {
        public int ProviderId { get; set; }
        public string InvoiceNumber { get; set; } = null!;
        public DateTime InvoiceDate { get; set; }
        public string Cai { get; set; } = null!;
        public int? PurchaseOrderOriginId { get; set; }
        public decimal? Exempt { get; set; }
        public decimal? Exonerated { get; set; }
        public decimal? TaxedAt15Percent { get; set; }
        public decimal? TaxedAt18Percent { get; set; }
        public decimal? Taxes15Percent { get; set; }
        public decimal? Taxes18Percent { get; set; }
    }
    public class CreatePurchaseBillCommandHandler : IRequestHandler<CreatePurchaseBillCommand,Response<PurchaseBillDto>>
    {
        private readonly IRepositoryAsync<PurchaseBill> _repositoryAsync;
        private readonly IRepositoryAsync<PurchaseOrder> _purchaseOrderRepositoryAsync;
        private readonly IRepositoryAsync<Provider> _providerRepositoryAsync;
        private readonly IMapper _mapper;
        public CreatePurchaseBillCommandHandler(IRepositoryAsync<PurchaseBill> repositoryAsync, IRepositoryAsync<PurchaseOrder> purchaseOrderRepositoryAsync, IRepositoryAsync<Provider> providerRepositoryAsync, IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _purchaseOrderRepositoryAsync = purchaseOrderRepositoryAsync;
            _providerRepositoryAsync = providerRepositoryAsync;
            _mapper = mapper;
        }
        public async Task<Response<PurchaseBillDto>> Handle(CreatePurchaseBillCommand request, CancellationToken cancellationToken)
        {
            if(request.PurchaseOrderOriginId!=null)
            {
                var purchaseOrderExist = await _purchaseOrderRepositoryAsync.FirstOrDefaultAsync(new FilterPurchaseOrderByIdSpecification((int)request.PurchaseOrderOriginId));
                if (purchaseOrderExist == null)
                {
                    throw new ApiException($"No existe una orden de compra con el Id {request.PurchaseOrderOriginId}");
                }
            }
            var providerExist = await _providerRepositoryAsync.FirstOrDefaultAsync(new FilterProviderSpecification(null,request.PurchaseOrderOriginId));
            if (providerExist == null)
            {
                throw new ApiException($"No existe una proveedor con el Id {request.ProviderId}");
            }
            var newRecord = _mapper.Map<PurchaseBill>(request);
            newRecord.ProviderId = request.ProviderId;
            newRecord.PurchaseOrderOriginId = request.PurchaseOrderOriginId;
            newRecord.StatusId = 27;
            newRecord.Total = (decimal)(request.Exempt + request.Exonerated + request.Taxes15Percent + request.Taxes18Percent + request.TaxedAt15Percent + request.TaxedAt18Percent);
            newRecord.CreationDate = DateTime.Now;
            var purchaseBillResponse = await _repositoryAsync.AddAsync(newRecord);
            await _repositoryAsync.SaveChangesAsync();
            var dto = _mapper.Map<PurchaseBillDto>(purchaseBillResponse);
            return new Response<PurchaseBillDto>(dto, $"Factura de compra creada exitosamente.");
        }
    }
}
