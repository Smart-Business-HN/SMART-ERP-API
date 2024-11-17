using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.ProductToPurchase;
using SMART.ERP.Application.DTOs.PurchaseOrder;
using SMART.ERP.Application.DTOs.Quotation;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.ProductToPurchaseSpecification;
using SMART.ERP.Application.Specifications.PurchaseOrderSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.PurchaseOrderFeature.Commands.UpdatePurchaseOrderCommand
{
    public class UpdatePurchaseOrderCommand : IRequest<Response<PurchaseOrderDto>>
    {
        public int Id { get; set; }
        public int ProviderId { get; set; }
        public int BranchOfficeId { get; set; }
        public Guid? UserId { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? DueDate { get; set; }
        public string? Observations { get; set; }
        public string? TermsAndConditions { get; set; }
        public List<ProductToBuyDto> ProductsToBuy { get; set; } = null!;
        public List<ProductToPurchaseDto> ProductsToPurchase { get; set; } = null!;
        public int StatusId { get; set; }
        public int PrefixId { get; set; }
    }
    public class UpdatePurchaseOrderCommandHandler : IRequestHandler<UpdatePurchaseOrderCommand, Response<PurchaseOrderDto>>
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
        public UpdatePurchaseOrderCommandHandler(IRepositoryAsync<PurchaseOrder> repositoryAsync, IMapper mapper, IRepositoryAsync<Provider> providerRepositoryAsync, IRepositoryAsync<BranchOffices> branchOfficeRepositoryAsync, IRepositoryAsync<User> userRepositoryAsync, IRepositoryAsync<Status> statusRepositoryAsync, IRepositoryAsync<Tax> taxRepositoryAsync, IRepositoryAsync<Product> productRepositoryAsync, IRepositoryAsync<Prefix> prefixRepositoryAsync, IRepositoryAsync<ProductToPurchase> productToPurchaseRepositoryAsync)
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
        public async Task<Response<PurchaseOrderDto>> Handle(UpdatePurchaseOrderCommand request, CancellationToken cancellationToken)
        {
            var purhaseOrderExist = await _repositoryAsync.FirstOrDefaultAsync(new FilterPurchaseOrderByIdSpecification(request.Id));
            if (purhaseOrderExist == null)
            {
                throw new ApiException($"No existe una order de compra con el Id {request.Id}");
            }
            var providerExist = await _providerRepositoryAsync.GetByIdAsync(request.ProviderId);
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
            //UPDATING VALUES
            if (purhaseOrderExist.ProviderId != request.ProviderId)
            {
                purhaseOrderExist.ProviderId = request.ProviderId;
                purhaseOrderExist.Provider = null;
            }
            if (purhaseOrderExist.BranchOfficeId != request.BranchOfficeId)
            {
                purhaseOrderExist.BranchOfficeId = request.BranchOfficeId;
            }
            if (purhaseOrderExist.StatusId != request.StatusId)
            {
                purhaseOrderExist.StatusId = request.StatusId;
            }
            if (purhaseOrderExist.PrefixId != request.PrefixId)
            {
                purhaseOrderExist.PrefixId = request.PrefixId;
            }
            if (purhaseOrderExist.Observations != request.Observations)
            {
                purhaseOrderExist.Observations = request.Observations;
            }
            if (purhaseOrderExist.TermsAndConditions != request.TermsAndConditions)
            {
                purhaseOrderExist.TermsAndConditions = request.TermsAndConditions;
            }
            if (request.DueDate != null && purhaseOrderExist.DueDate != request.DueDate)
            {
                purhaseOrderExist.DueDate = (DateTime)request.DueDate;
            }
            //PRODUCS to offered
            var taxesInDatabase = await _taxRepositoryAsync.ListAsync();
            var pre = await CheckProducts(request.ProductsToPurchase, request.ProductsToBuy, request.Id, taxesInDatabase);
            purhaseOrderExist.Subtotal = CalculateSubtotal(pre);
            var taxes = CalculateTaxes(pre, taxesInDatabase);
            decimal taxesAmount = 0;
            foreach (var taxis in taxes)
            {
                taxesAmount += taxis.Amount;
            }
            purhaseOrderExist.Total = taxesAmount + purhaseOrderExist.Subtotal;
            purhaseOrderExist.ProductsToPurchase = null;
            await _repositoryAsync.UpdateAsync(purhaseOrderExist);
            await _repositoryAsync.SaveChangesAsync();
            purhaseOrderExist.Provider = providerExist;
            var dto = _mapper.Map<PurchaseOrderDto>(purhaseOrderExist);
            dto.ProductsToPurchase = pre;
            return new Response<PurchaseOrderDto>(dto, $"Cotizacion {purhaseOrderExist.PurchaseOrderCode} actualizada exitosamente.");
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
        public async Task<List<ProductToPurchaseDto>> CheckProducts(List<ProductToPurchaseDto> productsToPurchase, List<ProductToBuyDto> productsToBuy, int purchaseOrderId, List<Tax> taxesRates)
        {
            var productsToProcess = new List<ProductToBuyDto>(productsToBuy);
            var prouductsPreExistence = new List<ProductToPurchaseDto>(productsToPurchase);

            foreach (var productExisting in productsToPurchase)
            {
                //UPDATING EVERY PRODUCT AND REMOVING FROM LOCAL LISTS
                foreach (var item in productsToBuy)
                {
                    if (productExisting.ProductId == item.ProductId)
                    {
                        var productToUpdate = productExisting;
                        if (productToUpdate.Quantity != item.Quantity)
                        {
                            productToUpdate.Quantity = item.Quantity;
                        }
                        if (productToUpdate.UnitPrice != item.RecomendedBuyPrice)
                        {
                            productToUpdate.UnitPrice = item.RecomendedBuyPrice;
                        }
                        productToUpdate.TotalLine = item.Quantity * item.RecomendedBuyPrice;
                        productToUpdate.Taxes = TaxCalculator(item, taxesRates);
                        productToUpdate.Product = null;
                        productToUpdate.Tax = null;
                        productToUpdate.PurchaseOrder = null;
                        var productSeed = _mapper.Map<ProductToPurchase>(productToUpdate);
                        await _productToPurchaseRepositoryAsync.UpdateAsync(productSeed);
                        await _productToPurchaseRepositoryAsync.SaveChangesAsync();
                        prouductsPreExistence.RemoveAll(x => x.ProductId == productExisting.ProductId);
                        productsToProcess.RemoveAll(x => x.ProductId == item.ProductId);
                    }
                }
            }
            //ADD NEW PRODUCTS TO THE PURCHASE ORDER
            foreach (var newProductToQuote in productsToProcess)
            {
                var newProductOffered = _mapper.Map<ProductToPurchase>(newProductToQuote);
                newProductOffered.PurchaseOrderId = purchaseOrderId;
                newProductOffered.UnitPrice = newProductToQuote.RecomendedBuyPrice;
                newProductOffered.Taxes = TaxCalculator(newProductToQuote, taxesRates);
                newProductOffered.TotalLine = newProductToQuote.Quantity * newProductToQuote.RecomendedBuyPrice;
                await _productToPurchaseRepositoryAsync.AddAsync(newProductOffered);
                await _productToPurchaseRepositoryAsync.SaveChangesAsync();
            }
            //REMOVING PREVIOUS PRODUCTS FROM THE QUOTATION
            foreach (var productPreExistence in prouductsPreExistence)
            {
                productPreExistence.Tax = null;
                productPreExistence.PurchaseOrder = null;
                productPreExistence.Product = null;

                var dtoToDelete = _mapper.Map<ProductToPurchase>(productPreExistence);
                await _productToPurchaseRepositoryAsync.DeleteAsync(dtoToDelete);
                await _productToPurchaseRepositoryAsync.SaveChangesAsync();
            }
            //GETS NEW PRODUCTS FROM THE PURCHASE ORDER
            var newProducts = await _productToPurchaseRepositoryAsync.ListAsync(new ProductToPurchaseSpecification(purchaseOrderId));
            var dtos = _mapper.Map<List<ProductToPurchaseDto>>(newProducts);

            return dtos;
        }
        static public decimal TaxCalculator(ProductToBuyDto product, List<Tax> taxes)
        {
            Tax productTax = taxes.Find(x => x.Id == product.TaxId)!;
            decimal gravable = product.Quantity * product.RecomendedBuyPrice;
            decimal total = gravable * ((productTax!.Rate / 100) + 1);
            decimal tax = total - gravable;
            return tax;
        }
    }
}
