using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Quotation;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.ClientSpecification;
using SMART.ERP.Application.Specifications.ProductOfferedSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.QuotationFeature.Commands.CopyQuotationFromIdCommand
{
    public class CopyQuotationFromIdCommand : IRequest<Response<int>>
    {
        public int? Id { get; set; }
        public Guid CustomerId { get; set; }
        public int BranchOfficeId { get; set; }
        public Guid? UserId { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? DueDate { get; set; }
        public string? Observations { get; set; }
        public string? TermsAndConditions { get; set; }
        public List<ProductOfferedDto> ProductsOffered { get; set; } = null!;
        public int StatusId { get; set; }
        public int PrefixId { get; set; }
    }
    public class CopyQuotationFromIdCommandHandler : IRequestHandler<CopyQuotationFromIdCommand,Response<int>>
    {
        private readonly IRepositoryAsync<Quotation> _repositoryAsync;
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<Customer> _customerRepositoryAsync;
        private readonly IRepositoryAsync<BranchOffices> _branchOfficeRepositoryAsync;
        private readonly IRepositoryAsync<User> _userRepositoryAsync;
        private readonly IRepositoryAsync<Status> _statusRepositoryAsync;
        private readonly IRepositoryAsync<Tax> _taxRepositoryAsync;
        private readonly IRepositoryAsync<Prefix> _prefixRepositoryAsync;
        private readonly IRepositoryAsync<ProductOffered> _productOfferedRepositoryAsync;
        public CopyQuotationFromIdCommandHandler(IRepositoryAsync<Quotation> repositoryAsync, IMapper mapper, IRepositoryAsync<Customer> customerRepositoryAsync, IRepositoryAsync<BranchOffices> branchOfficeRepositoryAsync, IRepositoryAsync<User> userRepositoryAsync, IRepositoryAsync<Status> statusRepositoryAsync, IRepositoryAsync<Tax> taxRepositoryAsync, IRepositoryAsync<Prefix> prefixRepositoryAsync, IRepositoryAsync<ProductOffered> productOfferedRepositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
            _mapper = mapper;
            _customerRepositoryAsync = customerRepositoryAsync;
            _branchOfficeRepositoryAsync = branchOfficeRepositoryAsync;
            _userRepositoryAsync = userRepositoryAsync;
            _statusRepositoryAsync = statusRepositoryAsync;
            _taxRepositoryAsync = taxRepositoryAsync;
            _prefixRepositoryAsync = prefixRepositoryAsync;
            _productOfferedRepositoryAsync = productOfferedRepositoryAsync;
        }
       public async Task<Response<int>> Handle(CopyQuotationFromIdCommand request, CancellationToken cancellationToken)
        {
            var customerExist = await _customerRepositoryAsync.FirstOrDefaultAsync(new FilterClientByIdSpecification(request.CustomerId));
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
            var taxesRates = await _taxRepositoryAsync.ListAsync();
            var productsOffered = new List<ProductOfferedDto>();
            var currentCuotations = await _repositoryAsync.ListAsync();
            var productsInRequest = request.ProductsOffered;
            request.Id = null;
            request.ProductsOffered = null;
            request.CreationDate = DateTime.Now;
            var newRecord = _mapper.Map<Quotation>(request);
            //TODO: refactorizin this to: print a different correlative for each prefix.
            newRecord.QuotationCode = CreateQuotationCode(prefixExist, currentCuotations.Last());
            newRecord.Profitability = 0;
            newRecord.SubTotal = 0;
            newRecord.Total = 0;
            var quoteResponse = await _repositoryAsync.AddAsync(newRecord);
            await _repositoryAsync.SaveChangesAsync();
            if (productsInRequest != null && productsInRequest.Count > 0)
            {
                foreach (var item in productsInRequest)
                {
                    var product = new ProductOffered
                    {
                        QuotationId = quoteResponse.Id,
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice,
                        TaxId = item.TaxId,
                        Taxes = item.Taxes,
                        TotalLine = item.TotalLine,
                        ProductCode = item.ProductCode,
                        ProductName = item.ProductName,

                    };
                    var productOfferedResponse = await _productOfferedRepositoryAsync.AddAsync(product);
                    await _productOfferedRepositoryAsync.SaveChangesAsync();
                    var newProductOfferedDto = _mapper.Map<ProductOfferedDto>(productOfferedResponse);
                    productsOffered.Add(newProductOfferedDto);
                }
                newRecord.SubTotal = CalculateSubtotal(productsOffered);
                var taxes = CalculateTaxes(productsOffered, taxesRates);
                decimal taxesAmount = 0;
                foreach (var taxis in taxes)
                {
                    taxesAmount += taxis.Amount;
                }
                newRecord.Total = taxesAmount + newRecord.SubTotal;
                await _repositoryAsync.UpdateAsync(newRecord);
                await _repositoryAsync.SaveChangesAsync();
                quoteResponse.Total = newRecord.Total;
                quoteResponse.SubTotal = newRecord.SubTotal;
            }
            var productsForNewQuotation = await _productOfferedRepositoryAsync.ListAsync(new ProductOfferedSpecification(quoteResponse.Id));
            var productsDto = _mapper.Map<List<ProductOfferedDto>>(productsForNewQuotation);
            var dto = _mapper.Map<QuotationDto>(quoteResponse);
            dto.ProductsOffered = productsDto;
            return new Response<int>(dto.Id, $"Cotización {dto.QuotationCode} creada exitosamente.");
        }
        public static string CreateQuotationCode(Prefix prefix, Quotation lastQuotation)
        {
            var numberOfCharacters = prefix.Format.ToCharArray().Length;
            var numberOfCharactersInId = lastQuotation.Id.ToString().ToCharArray().Length;
            var code = "";
            if (numberOfCharacters + numberOfCharactersInId < 8)
            {
                int characterOffset = 8 - (numberOfCharacters + numberOfCharactersInId);
                code = prefix.Format + new string('0', characterOffset) + (lastQuotation.Id + 1).ToString();
            }
            else
            {
                code = prefix.Format + (lastQuotation.Id + 1).ToString();
            }
            return code;
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
        public List<KindOfTaxDto> CalculateTaxes(List<ProductOfferedDto> products, List<Tax> taxes)
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
                        tax = taxes.Find(x => x.Id == item);
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
    }
}
