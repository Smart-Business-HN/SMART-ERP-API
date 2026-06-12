using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.PurchaseBill;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.AccountingPostingService;
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
        private readonly IRepositoryAsync<ProductPurchasePriceLog> _productPurchasePriceLogRepositoryAsync;
        private readonly IRepositoryAsync<Product> _productRepositoryAsync;
        private readonly IMapper _mapper;
        private readonly IAccountingPostingService _accountingPostingService;

        public CreatePurchaseBillFromPurchaseOrderDetailPageCommandHandler(IRepositoryAsync<Product> productRepositoryAsync, IRepositoryAsync<ProductPurchasePriceLog> productPurchasePriceLogRepositoryAsync, IRepositoryAsync<PurchaseBill> repositoryAsync, IRepositoryAsync<Prefix> prefixRepositoryAsync, IRepositoryAsync<PurchaseOrder> purchaseOrderRepositoryAsync, IMapper mapper, IAccountingPostingService accountingPostingService)
        {
            _repositoryAsync = repositoryAsync;
            _purchaseOrderRepositoryAsync = purchaseOrderRepositoryAsync;
            _mapper = mapper;
            _prefixRepositoryAsync = prefixRepositoryAsync;
            _productPurchasePriceLogRepositoryAsync = productPurchasePriceLogRepositoryAsync;
            _productRepositoryAsync = productRepositoryAsync;
            _accountingPostingService = accountingPostingService;
        }
        public async Task<Response<PurchaseBillDto>> Handle(CreatePurchaseBillFromPurchaseOrderDetailPageCommand request, CancellationToken cancellationToken)
        {
            var purchaseOrderExist = await _purchaseOrderRepositoryAsync.FirstOrDefaultAsync(new FilterPurchaseOrderByIdSpecification(request.PurchaseOrderOriginId));
            if (purchaseOrderExist == null)
            {
                throw new ApiException($"No existe una orden de compra con el Id {request.PurchaseOrderOriginId}");
            }
            var prefixExist = await _prefixRepositoryAsync.GetByIdAsync(request.PrefixId);
            if (prefixExist == null)
            {
                throw new ApiException($"No existe un prefijo con el id {request.PrefixId}");
            }
            var currentPurchaseBills = await _repositoryAsync.ListAsync();
            var existPurchaseBill = currentPurchaseBills.Where(x => (x.Cai == request.Cai && x.InvoiceNumber == request.InvoiceNumber) || (x.ProviderId == purchaseOrderExist.ProviderId && x.InvoiceNumber == request.InvoiceNumber));
            if (existPurchaseBill.Any())
            {
                throw new ApiException($"Ya existe una factura con este numero, CAI y/o proveedor registrada. Favor revisar factura con ID {existPurchaseBill.First().Id}");
            }
            var newRecord = _mapper.Map<PurchaseBill>(request);
            newRecord.PurchaseBillCode = CreatePurchaseBillCode(prefixExist, currentPurchaseBills.Last());
            newRecord.PurchaseOrderOriginId = request.PurchaseOrderOriginId;
            newRecord.ProviderId = purchaseOrderExist.ProviderId;
            newRecord.StatusId = 27;
            newRecord.ExpenseAccountId = 1;
            newRecord.Total = (decimal)(request.Exempt + request.Exonerated + request.Taxes15Percent + request.Taxes18Percent + request.TaxedAt15Percent + request.TaxedAt18Percent)!;
            newRecord.Outstanding = newRecord.Total;
            newRecord.CreationDate = DateTime.UtcNow;
            var purchaseBillResponse = await _repositoryAsync.AddAsync(newRecord);
            await _repositoryAsync.SaveChangesAsync();
            await _accountingPostingService.PostPurchaseBillAsync(purchaseBillResponse.Id, cancellationToken);
            await SaveProductPurchasePriceLogs(purchaseOrderExist.ProductsToPurchase!, purchaseBillResponse);
            purchaseOrderExist.PurchaseBillDestinationId = purchaseBillResponse.Id;
            purchaseOrderExist.Prefix = null;
            purchaseOrderExist.Provider = null;
            purchaseOrderExist.ProductsToPurchase = null;
            // La recepción puede venir del flujo legacy (InventoryInput) o del nuevo (InventoryEntry).
            // Si ya está recibida por cualquiera de los dos, la orden queda Completa (24); si no, Facturada / Sin Recibir (22).
            bool yaRecibida = purchaseOrderExist.InventoryInputDestinationId != null
                           || purchaseOrderExist.InventoryEntryDestinationId != null;
            purchaseOrderExist.StatusId = yaRecibida ? 24 : 22;
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
        public async Task SaveProductPurchasePriceLogs(List<ProductToPurchase> products, PurchaseBill purchaseBill)
        {
            foreach (var product in products)
            {
                var productToUpdate = product.Product;
                productToUpdate!.CostPrice = product.UnitPrice;
                productToUpdate.RecomendedSalePrice = (Math.Ceiling(product.UnitPrice * (decimal)(1 + 0.18 + (double)(product.Tax!.Rate / 100)) / 5) * 5) / (1 + (product.Tax.Rate / 100));
                var newRecord = new ProductPurchasePriceLog
                {
                    ProductId = product.ProductId!.Value,
                    UnitsPurchased = product.Quantity,
                    Price = product.UnitPrice,
                    PurchaseDate = purchaseBill.InvoiceDate,
                    PurchaseBillOriginId = purchaseBill.Id
                };
                productToUpdate.Brand = null;
                productToUpdate.Tax = null;
                productToUpdate.ProductPurchasePriceLogs = null;
                await _productRepositoryAsync.UpdateAsync(productToUpdate);
                await _productPurchasePriceLogRepositoryAsync.AddAsync(newRecord);
            }
            await _productPurchasePriceLogRepositoryAsync.SaveChangesAsync();
            await _productRepositoryAsync.SaveChangesAsync();
        }
    }
}
