using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.PurchaseBill;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.PurchaseOrderSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.PurchaseBillFeature.Commands.CreatePurchaseBillFromPurchaseOrderDetailPageCommand
{
    public class CreatePurchaseBillFromPurchaseOrderDetailPageCommand : IRequest<Response<PurchaseBillDto>>
    {
        public int PurchaseOrderOriginId { get; set; }
        public string InvoiceNumber { get; set; } = null!;
        public DateTime InvoiceDate { get; set; }
        public string Cai { get; set; } = null!;
        public decimal? Exempt { get; set; }
        public decimal? Exonerated { get; set; }
        public decimal? TaxedAt15Percent { get; set; }
        public decimal? TaxedAt18Percent { get; set; }
        public decimal? Taxes15Percent { get; set; }
        public decimal? Taxes18Percent { get; set; }
        public int PrefixId { get; set; }
    }
    public class CreatePurchaseBillFromPurchaseOrderDetailPageCommandHandler : IRequestHandler<CreatePurchaseBillFromPurchaseOrderDetailPageCommand, Response<PurchaseBillDto>>
    {
        private readonly IRepositoryAsync<PurchaseBill> _repositoryAsync;
        private readonly IRepositoryAsync<PurchaseOrder> _purchaseOrderRepositoryAsync;
        private readonly IRepositoryAsync<Prefix> _prefixRepositoryAsync;
        private readonly IMapper _mapper;

        public CreatePurchaseBillFromPurchaseOrderDetailPageCommandHandler(IRepositoryAsync<PurchaseBill> repositoryAsync,IRepositoryAsync<Prefix> prefixRepositoryAsync, IRepositoryAsync<PurchaseOrder> purchaseOrderRepositoryAsync, IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _purchaseOrderRepositoryAsync = purchaseOrderRepositoryAsync;
            _mapper = mapper;
            _prefixRepositoryAsync = prefixRepositoryAsync;
        }
        public async Task<Response<PurchaseBillDto>> Handle(CreatePurchaseBillFromPurchaseOrderDetailPageCommand request, CancellationToken cancellationToken)
        {
            var purchaseOrderExist = await _purchaseOrderRepositoryAsync.FirstOrDefaultAsync(new FilterPurchaseOrderByIdSpecification(request.PurchaseOrderOriginId));
            if(purchaseOrderExist == null)
            {
                throw new ApiException($"No existe una orden de compra con el Id {request.PurchaseOrderOriginId}");
            }
            var prefixExist = await _prefixRepositoryAsync.GetByIdAsync(request.PrefixId);
            if (prefixExist == null)
            {
                throw new ApiException($"No existe un prefijo con el id {request.PrefixId}");
            }
            var currentPurchaseBills = await _repositoryAsync.ListAsync();
            var newRecord = _mapper.Map<PurchaseBill>(request);
            newRecord.PurchaseBillCode = CreatePurchaseBillCode(prefixExist, currentPurchaseBills.Last());
            newRecord.PurchaseOrderOriginId = request.PurchaseOrderOriginId;
            newRecord.ProviderId = purchaseOrderExist.ProviderId;
            newRecord.StatusId = 27;
            newRecord.Total = (decimal)(request.Exempt + request.Exonerated + request.Taxes15Percent + request.Taxes18Percent + request.TaxedAt15Percent + request.TaxedAt18Percent);
            newRecord.Outstanding = newRecord.Total;
            newRecord.CreationDate = DateTime.Now;
            var purchaseBillResponse = await _repositoryAsync.AddAsync(newRecord);
            await _repositoryAsync.SaveChangesAsync();
            purchaseOrderExist.PurchaseBillDestinationId = purchaseBillResponse.Id;
            purchaseOrderExist.Prefix = null;
            purchaseOrderExist.Provider = null;
            purchaseOrderExist.ProductsToPurchase = null;
            purchaseOrderExist.Status = null;
            purchaseOrderExist.BranchOffice = null;
            purchaseOrderExist.User = null;
            await _purchaseOrderRepositoryAsync.UpdateAsync(purchaseOrderExist);
            await _purchaseOrderRepositoryAsync.SaveChangesAsync();
            var dto = _mapper.Map<PurchaseBillDto>(purchaseBillResponse);
            return new Response<PurchaseBillDto>(dto, $"Factura de compra para orden de compra {purchaseOrderExist.PurchaseOrderCode} creada exitosamente.");
        }
        public static string CreatePurchaseBillCode(Prefix prefix, PurchaseBill lastPurchaseBill)
        {
            var numberOfCharacters = prefix.Format.ToCharArray().Length;
            var numberOfCharactersInId = lastPurchaseBill.Id.ToString().ToCharArray().Length;
            var code = "";
            if (numberOfCharacters + numberOfCharactersInId < 8)
            {
                int characterOffset = 8 - (numberOfCharacters + numberOfCharactersInId);
                code = prefix.Format + new string('0', characterOffset) + (lastPurchaseBill.Id + 1).ToString();
            }
            else
            {
                code = prefix.Format + (lastPurchaseBill.Id + 1).ToString();
            }
            return code;
        }
    }
}
