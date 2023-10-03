using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Quotation;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.QuotationSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Application.Specifications.ProductOfferedSpecification;


namespace SMART.ERP.Application.Features.QuotationFeature.Commands.UpdateQuotationCommand
{
    public class UpdateQuotationCommand : IRequest<Response<QuotationDto>>
    {
        public int Id { get; set; }
        public Guid CustomerId { get; set; }
        public int BranchOfficeId { get; set; }
        public Guid? UserId { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? DueDate { get; set; }
        public string? Observations { get; set; }
        public string? TermsAndConditions { get; set; }
        public List<ProductToOfferdDto> ProductsToOffered { get; set; } = null!;
        public List<ProductOfferedDto> ProductsOffered { get; set; } = null!;
        public int StatusId { get; set; }
        public int PrefixId { get; set; }
    }
    public class UpdateQuotationCommandHandler: IRequestHandler<UpdateQuotationCommand,Response<QuotationDto>>
    {
        private readonly IRepositoryAsync<Quotation> _repositoryAsync;
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<Customer> _customerRepositoryAsync;
        private readonly IRepositoryAsync<BranchOffices> _branchOfficeRepositoryAsync;
        private readonly IRepositoryAsync<User> _userRepositoryAsync;
        private readonly IRepositoryAsync<Status> _statusRepositoryAsync;
        private readonly IRepositoryAsync<Tax> _taxRepositoryAsync;
        private readonly IRepositoryAsync<Product> _productRepositoryAsync;
        private readonly IRepositoryAsync<Prefix> _prefixRepositoryAsync;
        private readonly IRepositoryAsync<ProductOffered> _productOfferedRepositoryAsync;
        public UpdateQuotationCommandHandler(IRepositoryAsync<Quotation> repositoryAsync, IMapper mapper, IRepositoryAsync<Customer> customerRepositoryAsync, IRepositoryAsync<BranchOffices> branchOfficeRepositoryAsync, IRepositoryAsync<User> userRepositoryAsync, IRepositoryAsync<Status> statusRepositoryAsync, IRepositoryAsync<Tax> taxRepositoryAsync, IRepositoryAsync<Product> productRepositoryAsync, IRepositoryAsync<Prefix> prefixRepositoryAsync, IRepositoryAsync<ProductOffered> productOfferedRepositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
            _mapper = mapper;
            _customerRepositoryAsync = customerRepositoryAsync;
            _branchOfficeRepositoryAsync = branchOfficeRepositoryAsync;
            _userRepositoryAsync = userRepositoryAsync;
            _statusRepositoryAsync = statusRepositoryAsync;
            _taxRepositoryAsync = taxRepositoryAsync;
            _productRepositoryAsync = productRepositoryAsync;
            _prefixRepositoryAsync = prefixRepositoryAsync;
            _productOfferedRepositoryAsync = productOfferedRepositoryAsync;
        }
        public async Task<Response<QuotationDto>> Handle(UpdateQuotationCommand request, CancellationToken cancellationToken)
        {
            var quotationExist = await _repositoryAsync.FirstOrDefaultAsync(new FilterQuotationByIdSpecification(request.Id));
            if (quotationExist == null)
            {
                throw new ApiException($"No existe una cotización con el Id {request.Id}");
            }
            var customerExist = await _customerRepositoryAsync.GetByIdAsync(request.CustomerId);
            if (customerExist == null)
            {
                throw new ApiException($"No existe un cliente con el Id {request.CustomerId}");
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
            if (quotationExist.CustomerId != request.CustomerId)
            {
                quotationExist.CustomerId = request.CustomerId;
            }
            if (quotationExist.BranchOfficeId != request.BranchOfficeId)
            {
                quotationExist.BranchOfficeId = request.BranchOfficeId;
            }
            if (quotationExist.StatusId != request.StatusId)
            {
                quotationExist.StatusId = request.StatusId;
            }
            if(quotationExist.PrefixId != request.PrefixId)
            {
                quotationExist.PrefixId = request.PrefixId;
            }
            if (quotationExist.Observations != request.Observations)
            {
                quotationExist.Observations = request.Observations;
            }
            if (quotationExist.TermsAndConditions != request.TermsAndConditions)
            {
                quotationExist.TermsAndConditions = request.TermsAndConditions;
            }
            //PRODUCS to offered
            var pre = await CheckProducts(request.ProductsOffered, request.ProductsToOffered, request.Id);
            quotationExist.SubTotal = CalculateSubtotal(pre);
            var taxes = await CalculateTaxes(pre);
            decimal taxesAmount = 0;
            foreach (var taxis in taxes)
            {
                taxesAmount += taxis.Amount;
            }
            quotationExist.Total = taxesAmount + quotationExist.SubTotal;
            quotationExist.ProductsOffered = null;
            await _repositoryAsync.UpdateAsync(quotationExist);
            await _repositoryAsync.SaveChangesAsync();
            var dto = _mapper.Map<QuotationDto>(quotationExist);
            dto.ProductsOffered = pre;
            return new Response<QuotationDto>(dto, $"Cotizacion {quotationExist.QuotationCode} actualizada exitosamente.");

        }
        public async Task<List<KindOfTaxDto>> CalculateTaxes(List<ProductOfferedDto> products)
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
                        tax = await _taxRepositoryAsync.GetByIdAsync(item);
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
        public async Task<List<ProductOfferedDto>> CheckProducts(List<ProductOfferedDto>productOffered,List<ProductToOfferdDto> productsToOffered, int quotationId)
        {
            var productsToProcess = new List<ProductToOfferdDto>(productsToOffered);
            var prouductsPreExistence = new List<ProductOfferedDto>(productOffered);
            var taxesRates = await _taxRepositoryAsync.ListAsync();
            for (int i = 0; i < productOffered.Count; i++)
            {
                for ( int e = 0; e < productsToOffered.Count; e++)
                {
                    if (productsToOffered[e].ProductId == productOffered[i].ProductId)
                    {
                        var product =  productOffered[i];
                        if(productsToOffered[e].Quantity != productOffered[i].Quantity )
                        {
                            product.Quantity = productsToOffered[e].Quantity;
                        }
                        if(productsToOffered[e].recomendedSalePrice != productOffered[i].UnitPrice)
                        {
                            product.UnitPrice = productsToOffered[e].recomendedSalePrice;
                        }
                        product.TotalLine = productsToOffered[e].Quantity * productsToOffered[e].recomendedSalePrice;
                        product.Taxes = TaxCalculator(productsToOffered[e], taxesRates);
                        product.Product = null;
                        product.Tax = null;
                        product.Quotation = null;
                        var productSeed = _mapper.Map<ProductOffered>(product);
                        await _productOfferedRepositoryAsync.UpdateAsync(productSeed);
                        await _productOfferedRepositoryAsync.SaveChangesAsync();
                        productsToProcess.RemoveAll(r => r.ProductId == productsToOffered[e].ProductId);
                        prouductsPreExistence.RemoveAll(r => r.Id == productOffered[i].Id);
                    }
                }
                if(prouductsPreExistence.Count > 0 )
                {
                    for(int a = 0; a<prouductsPreExistence.Count;a++)
                    {
                        var product = prouductsPreExistence[a];
                        product.Tax = null;
                        product.Quotation = null;
                        product.Product = null;
                        var dtoToDelete = _mapper.Map<ProductOffered>(product);
                        await _productOfferedRepositoryAsync.DeleteAsync(dtoToDelete);
                        await _productOfferedRepositoryAsync.SaveChangesAsync();
                    }
                }
            }
            if(productsToProcess.Count > 0)
            {
                foreach (var item in productsToProcess)
                {
                    var newProductOffered = _mapper.Map<ProductOffered>(item);
                    newProductOffered.QuotationId = quotationId;
                    newProductOffered.UnitPrice = item.recomendedSalePrice;
                    newProductOffered.Taxes = TaxCalculator(item, taxesRates);
                    newProductOffered.TotalLine = item.Quantity * item.recomendedSalePrice;
                    await _productOfferedRepositoryAsync.AddAsync(newProductOffered);
                    await _productOfferedRepositoryAsync.SaveChangesAsync();
                }
            }
            var newProducts = await _productOfferedRepositoryAsync.ListAsync(new ProductOfferedSpecification(quotationId));
            var dtos = _mapper.Map<List<ProductOfferedDto>>(newProducts);
            return dtos;
        }
        static public decimal TaxCalculator(ProductToOfferdDto product, List<Tax> taxes)
        {
            Tax productTax = null!;
            for (int i = 0; i < taxes.Count; i++)
            {
                if (product.TaxId == taxes[i].Id)
                {
                    productTax = taxes[i];
                }
            }
            decimal gravable = product.Quantity * product.recomendedSalePrice;
            decimal total = gravable * ((productTax!.Rate / 100) + 1);
            decimal tax = total - gravable;
            return tax;
        }
        static public decimal CalculateSubtotal(List<ProductOfferedDto> products)
        {
            decimal subtotal = 0;
            foreach (var item in products)
            {
                decimal v = item.Quantity * item.UnitPrice;
                subtotal += v;
            }
            return subtotal;
        }
    }
}
