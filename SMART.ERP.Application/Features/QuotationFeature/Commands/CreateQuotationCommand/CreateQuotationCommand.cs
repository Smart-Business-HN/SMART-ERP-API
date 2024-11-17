using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Quotation;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.JwtService;
using SMART.ERP.Application.Specifications.ClientSpecification;
using SMART.ERP.Application.Specifications.ProductOfferedSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.QuotationFeature.Commands.CreateQuotationCommand
{
    public class CreateQuotationCommand : IRequest<Response<QuotationDto>>
    {
        public Guid CustomerId { get; set; }
        public int BranchOfficeId { get; set; }
        public Guid? UserId { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? DueDate { get; set; }
        public string? Observations { get; set; }
        public string? TermsAndConditions { get; set; }
        public List<ProductToOfferdDto>? ProductsToOffered { get; set; }
        public int StatusId { get; set; }
        public int PrefixId { get; set; }
    }
    public class CreateQuotationCommandHandler : IRequestHandler<CreateQuotationCommand, Response<QuotationDto>>
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
        private readonly IJwtService _jwtService;
        private readonly IRepositoryAsync<Notification> _notificationRepositoryAsync;
        public CreateQuotationCommandHandler(IRepositoryAsync<Quotation> repositoryAsync, IRepositoryAsync<Notification> notificationRepositoryAsync, IJwtService jwtService, IRepositoryAsync<ProductOffered> productOfferedRepositoryAsync, IRepositoryAsync<Customer> customerRepositoryAsync, IMapper mapper, IRepositoryAsync<BranchOffices> branchOfficeRepositoryAsync, IRepositoryAsync<User> userRepositoryAsync, IRepositoryAsync<Status> statusRepositoryAsync, IRepositoryAsync<Tax> taxRepositoryAsync, IRepositoryAsync<Product> productRepositoryAsync, IRepositoryAsync<Prefix> prefixRepositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
            _mapper = mapper;
            _branchOfficeRepositoryAsync = branchOfficeRepositoryAsync;
            _userRepositoryAsync = userRepositoryAsync;
            _statusRepositoryAsync = statusRepositoryAsync;
            _taxRepositoryAsync = taxRepositoryAsync;
            _productRepositoryAsync = productRepositoryAsync;
            _prefixRepositoryAsync = prefixRepositoryAsync;
            _customerRepositoryAsync = customerRepositoryAsync;
            _productOfferedRepositoryAsync = productOfferedRepositoryAsync;
            _notificationRepositoryAsync = notificationRepositoryAsync;
            _jwtService = jwtService;
        }
        public async Task<Response<QuotationDto>> Handle(CreateQuotationCommand request, CancellationToken cancellationToken)
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
            if (request.ProductsToOffered != null)
            {

                var checkProducts = await CheckMasterDataOfProductsToOffered(request.ProductsToOffered, taxesRates);
                if (checkProducts != "true")
                {
                    throw new ApiException($"{checkProducts}");
                }
                var checkDescriptios = CheckDescriptionsAndNamesOfProductsToOffered(request.ProductsToOffered);
                if (checkDescriptios != "true")
                {
                    throw new ApiException($"{checkDescriptios}");
                }
            }
            var userExist = await _userRepositoryAsync.GetByIdAsync(request.UserId!.Value);
            if (userExist == null)
            {
                throw new ApiException($"No existe un usuario con el Id {request.CustomerId}");
            }
            var productsOffered = new List<ProductOfferedDto>();
            var currentCuotations = await _repositoryAsync.ListAsync();
            var newRecord = _mapper.Map<Quotation>(request);
            var lastQuotationId = currentCuotations.Count > 0 ? currentCuotations.Last().Id : 0;
            newRecord.QuotationCode = CreateQuotationCode(prefixExist, lastQuotationId);
            newRecord.Profitability = 0;
            newRecord.SubTotal = 0;
            newRecord.Total = 0;
            newRecord.CreatedBy = _jwtService.GetSubjectToken();
            newRecord.InsertedDate = DateTime.UtcNow;
            var quoteResponse = await _repositoryAsync.AddAsync(newRecord);
            await _repositoryAsync.SaveChangesAsync();
            if (request.ProductsToOffered != null && request.ProductsToOffered.Count > 0)
            {
                foreach (var item in request.ProductsToOffered)
                {
                    var newProductOffered = _mapper.Map<ProductOffered>(item);
                    newProductOffered.QuotationId = quoteResponse.Id;
                    newProductOffered.UnitPrice = item.RecomendedSalePrice;
                    newProductOffered.Taxes = TaxCalculator(item, taxesRates);
                    newProductOffered.TotalLine = newProductOffered.Taxes + (item.Quantity * item.RecomendedSalePrice);
                    var productOfferedResponse = await _productOfferedRepositoryAsync.AddAsync(newProductOffered);
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
            quoteResponse.Customer = customerExist;
            quoteResponse.User = userExist;
            if (userExist.FullName != _jwtService.GetSubjectToken())
            {
                var notification = new Notification();
                notification.Title = "Cotizacion Creada en tu nombre!";
                notification.Icon = "heroicons_outline:information-circle";
                notification.Description = $"El usuario <b>{_jwtService.GetSubjectToken()}</b> ha creado la cotizacion {quoteResponse.QuotationCode}. Verifica la informacion.";
                notification.Time = DateTime.Now;
                notification.UseRouter = true;
                notification.Link = "/accounting/quotations/" + quoteResponse.Id;
                notification.Read = false;
                notification.UserId = request.UserId!.Value;

                var response = await _notificationRepositoryAsync.AddAsync(notification);
                await _notificationRepositoryAsync.SaveChangesAsync();
            }
            var dto = _mapper.Map<QuotationDto>(quoteResponse);
            dto.ProductsOffered = productsDto;
            return new Response<QuotationDto>(dto, $"Cotización {dto.QuotationCode} creada exitosamente.");
        }
        static public decimal TaxCalculator(ProductToOfferdDto product, List<Tax> taxes)
        {
            Tax? productTax = null;
            productTax = taxes.Find(x => x.Id == product.TaxId)!;
            decimal gravable = product.Quantity * product.RecomendedSalePrice;
            decimal total = gravable * ((productTax.Rate / 100) + 1);
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
                        tax = taxes.Find(x => x.Id == item)!;
                        decimal subTotalAmount = product.Quantity * product.UnitPrice;
                        decimal rates = 1 + (tax!.Rate / 100);
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

        static public string CheckDescriptionsAndNamesOfProductsToOffered(List<ProductToOfferdDto> request)
        {
            foreach (var item in request)
            {
                if (item.ProductId == null && item.ProductDescription == null)
                {
                    return "El Producto y/o nombre del producto es requerido";
                }
            }
            return "true";
        }
        public static string CreateQuotationCode(Prefix prefix, int lastQuotationId)
        {
            var numberOfCharacters = prefix.Format.ToCharArray().Length;
            var numberOfCharactersInId = (lastQuotationId + 1).ToString().ToCharArray().Length;
            var code = "";
            if (numberOfCharacters + numberOfCharactersInId < 8)
            {
                int characterOffset = 8 - (numberOfCharacters + numberOfCharactersInId);
                code = prefix.Format + new string('0', characterOffset) + (lastQuotationId + 1).ToString();
            }
            else
            {
                code = prefix.Format + (lastQuotationId + 1).ToString();
            }
            return code;
        }
        public async Task<string> CheckMasterDataOfProductsToOffered(List<ProductToOfferdDto> request, List<Tax> taxes)
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
