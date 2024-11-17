using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.ProductToPurchase;
using SMART.ERP.Application.DTOs.PurchaseOrder;
using SMART.ERP.Application.DTOs.Quotation;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.ProductToPurchaseSpecification;
using SMART.ERP.Application.Specifications.ProviderSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.PurchaseOrderFeature.Commands.CreatePurchaseOrderCommand
{
    public class CreatePurchaseOrderCommand : IRequest<Response<PurchaseOrderDto>>
    {
        public int ProviderId { get; set; }
        public int BranchOfficeId { get; set; }
        public Guid? UserId { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? DueDate { get; set; }
        public string? Observations { get; set; }
        public string? TermsAndConditions { get; set; }
        public List<ProductToBuyDto>? ProductsToBuy { get; set; }
        public int StatusId { get; set; }
        public int PrefixId { get; set; }
    }
    public class CreatePurchaseOrderCommandHandler : IRequestHandler<CreatePurchaseOrderCommand, Response<PurchaseOrderDto>>
    {
        private readonly IRepositoryAsync<PurchaseOrder> _repositoryAsync;
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<Provider> _providerRepositoryAsync;
        private readonly IRepositoryAsync<BranchOffices> _branchOfficeRepositoryAsync;
        private readonly IRepositoryAsync<User> _userRepositoryAsync;
        private readonly IRepositoryAsync<Status> _statusRepositoryAsync;
        private readonly IRepositoryAsync<Tax> _taxRepositoryAsync;
        private readonly IRepositoryAsync<Product> _productRepositoryAsync;
        private readonly IRepositoryAsync<Prefix> _prefixRepositoryAsync;
        private readonly IRepositoryAsync<ProductToPurchase> _productToPurchaseRepositoryAsync;
        public CreatePurchaseOrderCommandHandler(IRepositoryAsync<PurchaseOrder> repositoryAsync, IMapper mapper, IRepositoryAsync<Provider> providerRepositoryAsync, IRepositoryAsync<BranchOffices> branchOfficeRepositoryAsync, IRepositoryAsync<User> userRepositoryAsync, IRepositoryAsync<Status> statusRepositoryAsync, IRepositoryAsync<Tax> taxRepositoryAsync, IRepositoryAsync<Product> productRepositoryAsync, IRepositoryAsync<Prefix> prefixRepositoryAsync, IRepositoryAsync<ProductToPurchase> productToPurchaseRepositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
            _mapper = mapper;
            _providerRepositoryAsync = providerRepositoryAsync;
            _branchOfficeRepositoryAsync = branchOfficeRepositoryAsync;
            _userRepositoryAsync = userRepositoryAsync;
            _statusRepositoryAsync = statusRepositoryAsync;
            _taxRepositoryAsync = taxRepositoryAsync;
            _productRepositoryAsync = productRepositoryAsync;
            _prefixRepositoryAsync = prefixRepositoryAsync;
            _productToPurchaseRepositoryAsync = productToPurchaseRepositoryAsync;
        }
        public async Task<Response<PurchaseOrderDto>> Handle(CreatePurchaseOrderCommand request, CancellationToken cancellationToken)
        {
            var providerExist = await _providerRepositoryAsync.FirstOrDefaultAsync(new FilterProviderSpecification(null, request.ProviderId));
            if (providerExist == null)
            {
                throw new ApiException($"No existe un proveedor con el Id {request.ProviderId}");
            }
            var branchOfficeExist = await _branchOfficeRepositoryAsync.GetByIdAsync(request.BranchOfficeId);
            if (branchOfficeExist == null)
            {
                throw new ApiException($"No existe una sucursal con el id {request.BranchOfficeId}");
            }
            var statusExist = await _statusRepositoryAsync.GetByIdAsync(request.StatusId);
            if (statusExist == null)
            {
                throw new ApiException($"No existe un Estado con el id {request.StatusId}");
            }
            var prefixExist = await _prefixRepositoryAsync.GetByIdAsync(request.PrefixId);
            if (prefixExist == null)
            {
                throw new ApiException($"No existe un prefijo con el id {request.PrefixId}");
            }
            var taxesRates = await _taxRepositoryAsync.ListAsync();
            if (request.ProductsToBuy != null)
            {

                var checkProducts = await CheckMasterDataOfProductsToBuy(request.ProductsToBuy, taxesRates);
                if (checkProducts != "true")
                {
                    throw new ApiException($"{checkProducts}");
                }
                var checkDescriptios = CheckDescriptionsAndNamesOfProductsToBuy(request.ProductsToBuy);
                if (checkDescriptios != "true")
                {
                    throw new ApiException($"{checkDescriptios}");
                }
            }
            var producsToPurchase = new List<ProductToPurchaseDto>();
            var currentPurchaseOrders = await _repositoryAsync.ListAsync();
            if (currentPurchaseOrders.Count() == 0)
            {
                var firstPurchaseOrder = new PurchaseOrder { Id = 0 };
                currentPurchaseOrders = [firstPurchaseOrder];

            }
            var newRecord = _mapper.Map<PurchaseOrder>(request);
            newRecord.PurchaseOrderCode = CreatePurchaseOrderCode(prefixExist, currentPurchaseOrders.Last());
            newRecord.Subtotal = 0;
            newRecord.Total = 0;
            var purchaseOrderResponse = await _repositoryAsync.AddAsync(newRecord);
            await _repositoryAsync.SaveChangesAsync();
            if (request.ProductsToBuy != null && request.ProductsToBuy.Count > 0)
            {
                foreach (var item in request.ProductsToBuy)
                {
                    var newProductToPurchase = _mapper.Map<ProductToPurchase>(item);
                    newProductToPurchase.PurchaseOrderId = purchaseOrderResponse.Id;
                    newProductToPurchase.UnitPrice = item.RecomendedBuyPrice;
                    newProductToPurchase.Taxes = TaxCalculator(item, taxesRates);
                    newProductToPurchase.TotalLine = newProductToPurchase.Taxes + (item.Quantity * item.RecomendedBuyPrice);
                    var productToPurchaseResponse = await _productToPurchaseRepositoryAsync.AddAsync(newProductToPurchase);
                    await _productToPurchaseRepositoryAsync.SaveChangesAsync();
                    var newProductOfferedDto = _mapper.Map<ProductToPurchaseDto>(productToPurchaseResponse);
                    producsToPurchase.Add(newProductOfferedDto);
                }
                newRecord.Subtotal = CalculateSubtotal(producsToPurchase);
                var taxes = CalculateTaxes(producsToPurchase, taxesRates);
                decimal taxesAmount = 0;
                foreach (var taxis in taxes)
                {
                    taxesAmount += taxis.Amount;
                }
                newRecord.Total = taxesAmount + newRecord.Subtotal;
                await _repositoryAsync.UpdateAsync(newRecord);
                await _repositoryAsync.SaveChangesAsync();
                purchaseOrderResponse.Total = newRecord.Total;
                purchaseOrderResponse.Subtotal = newRecord.Subtotal;
            }
            var productsForNewPurchaseORder = await _productToPurchaseRepositoryAsync.ListAsync(new ProductToPurchaseSpecification(purchaseOrderResponse.Id));
            var productsDto = _mapper.Map<List<ProductToPurchaseDto>>(productsForNewPurchaseORder);
            var dto = _mapper.Map<PurchaseOrderDto>(purchaseOrderResponse);
            dto.ProductsToPurchase = productsDto;
            return new Response<PurchaseOrderDto>(dto, $"Orden de compra {dto.PurchaseOrderCode} creada exitosamente.");
        }
        public List<KindOfTaxDto> CalculateTaxes(List<ProductToPurchaseDto> products, List<Tax> taxes)
        {
            List<KindOfTaxDto> Taxes = new List<KindOfTaxDto>();
            List<int> kindOfTaxes = new List<int>();
            foreach (var product in products)
            {
                if (!kindOfTaxes.Contains(product.TaxId))
                {
                    kindOfTaxes.Add(product.TaxId);
                }
            }
            foreach (var item in kindOfTaxes)
            {
                decimal currentTaxAmount = 0;
                KindOfTaxDto newTaxAmount = new KindOfTaxDto();
                Tax tax = new();
                foreach (var product in products)
                {
                    if (item == product.TaxId)
                    {
                        tax = taxes.Find(x => x.Id == item)!;
                        decimal subTotalAmount = product.Quantity * product.UnitPrice;
                        decimal rates = 1 + (tax.Rate / 100);
                        decimal totalAmountWithTaxes = subTotalAmount * rates;
                        currentTaxAmount += (totalAmountWithTaxes - subTotalAmount);
                    }
                }
                newTaxAmount.Tax = tax;
                newTaxAmount.Amount = currentTaxAmount;
                Taxes.Add(newTaxAmount);
            }
            return Taxes;
        }
        static public decimal CalculateSubtotal(List<ProductToPurchaseDto> products)
        {
            decimal subtotal = 0;
            foreach (var item in products)
            {
                decimal v = item.Quantity * item.UnitPrice;
                subtotal += v;
            }
            return subtotal;
        }
        static public decimal TaxCalculator(ProductToBuyDto product, List<Tax> taxes)
        {
            Tax? productTax = null;
            productTax = taxes.Find(x => x.Id == product.TaxId)!;
            decimal gravable = product.Quantity * product.RecomendedBuyPrice;
            decimal total = gravable * ((productTax.Rate / 100) + 1);
            decimal tax = total - gravable;
            return tax;
        }
        public static string CreatePurchaseOrderCode(Prefix prefix, PurchaseOrder lastPurchaseOrder)
        {
            var numberOfCharacters = prefix.Format.ToCharArray().Length;
            var numberOfCharactersInId = lastPurchaseOrder.Id.ToString().ToCharArray().Length;
            var code = "";
            if (numberOfCharacters + numberOfCharactersInId < 8)
            {
                int characterOffset = 8 - (numberOfCharacters + numberOfCharactersInId);
                code = prefix.Format + new string('0', characterOffset) + (lastPurchaseOrder.Id + 1).ToString();
            }
            else
            {
                code = prefix.Format + (lastPurchaseOrder.Id + 1).ToString();
            }
            return code;
        }
        static public string CheckDescriptionsAndNamesOfProductsToBuy(List<ProductToBuyDto> request)
        {
            foreach (var item in request)
            {
                if (item.ProductId == null && item.ProductName == null)
                {
                    return "El Producto y/o nombre del producto es requerido";
                }
            }
            return "true";
        }
        public async Task<string> CheckMasterDataOfProductsToBuy(List<ProductToBuyDto> request, List<Tax> taxes)
        {
            List<int> taxesIds = new();
            List<int> productsId = new();
            foreach (var productToOfferdDto in request)
            {
                if (!taxesIds.Contains(productToOfferdDto.TaxId))
                {
                    taxesIds.Add(productToOfferdDto.TaxId);
                }
                if (productToOfferdDto.ProductId != null)
                {
                    if (!productsId.Contains((int)productToOfferdDto.ProductId))
                    {
                        productsId.Add((int)productToOfferdDto.ProductId);
                    }
                }
            }
            if (taxesIds.Count > 0)
            {

                foreach (var tax in taxesIds)
                {
                    var currentTax = taxes.Exists(x => x.Id == tax);
                    if (currentTax == false)
                    {
                        return "Ha habido un problema con el Id del Impuesto de uno de los productos";
                    }
                }
            }
            if (productsId.Count > 0)
            {
                foreach (var product in productsId)
                {
                    var productExist = await _productRepositoryAsync.GetByIdAsync(product);
                    if (productExist == null)
                    {
                        return "Ha habido un problema con el Id de uno de los productos";
                    }
                }
            }
            return "true";
        }
    }
}
