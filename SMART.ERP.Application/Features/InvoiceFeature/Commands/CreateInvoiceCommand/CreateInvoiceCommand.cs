using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Invoice;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.ProductSoldSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;


namespace SMART.ERP.Application.Features.InvoiceFeature.Commands.CreateInvoiceCommand
{
    public class CreateInvoiceCommand : IRequest<Response<InvoiceDto>>
    {
        public Guid CustomerId { get; set; }
        public int CaiId { get; set; }
        public int BranchOfficeId { get; set; }
        public Guid? UserId { get; set; }
        public DateTime CreationDate { get; set; }
        public string? Observations { get; set; }
        public string? TermsAndConditions { get; set; }
        public List<ProductToSellDto>? ProductsToSell { get; set; }
        public int StatusId { get; set; }
        public string? PurchaseOrderCode { get; set; }
        public string? SagCode { get; set; }
        public string? ExemptOrderNumber { get; set; }
        public string? ExemptedRegistrationCertificateNumber { get; set; }
    }
    public class CreateInvoiceCommandHandler : IRequestHandler<CreateInvoiceCommand, Response<InvoiceDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<Invoice> _repositoryAsync;
        private readonly IRepositoryAsync<Cai> _caiRepositoryAsync;
        private readonly IRepositoryAsync<Customer> _customerRepositoryAsync;
        private readonly IRepositoryAsync<BranchOffices> _branchOfficeRepositoryAsync;
        private readonly IRepositoryAsync<User> _userRepositoryAsync;
        private readonly IRepositoryAsync<Status> _statusRepositoryAsync;
        private readonly IRepositoryAsync<Tax> _taxRepositoryAsync;
        private readonly IRepositoryAsync<Product> _productRepositoryAsync;
        private readonly IRepositoryAsync<ProductSold> _productSoldRepositoryAsync;
        public CreateInvoiceCommandHandler(IMapper mapper, IRepositoryAsync<Invoice> repositoryAsync, IRepositoryAsync<Cai> caiRepositoryAsync, IRepositoryAsync<Customer> customerRepositoryAsync, IRepositoryAsync<BranchOffices> branchOfficeRepositoryAsync, IRepositoryAsync<User> userRepositoryAsync, IRepositoryAsync<Status> statusRepositoryAsync, IRepositoryAsync<Tax> taxRepositoryAsync, IRepositoryAsync<Product> productRepositoryAsync, IRepositoryAsync<ProductSold> productSoldRepositoryAsync)
        {
            _mapper = mapper;
            _repositoryAsync = repositoryAsync;
            _caiRepositoryAsync = caiRepositoryAsync;
            _customerRepositoryAsync = customerRepositoryAsync;
            _branchOfficeRepositoryAsync = branchOfficeRepositoryAsync;
            _userRepositoryAsync = userRepositoryAsync;
            _statusRepositoryAsync = statusRepositoryAsync;
            _taxRepositoryAsync = taxRepositoryAsync;
            _productRepositoryAsync = productRepositoryAsync;
            _productSoldRepositoryAsync = productSoldRepositoryAsync;
        }
        public async Task<Response<InvoiceDto>> Handle(CreateInvoiceCommand request, CancellationToken cancellationToken)
        {
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
            var caiExist = await _caiRepositoryAsync.GetByIdAsync(request.CaiId);
            if (caiExist == null)
            {
                throw new ApiException($"No existe un CAI con el id {request.CaiId}");
            }
            if(!caiExist.IsActive)
            {
                throw new ApiException($"El CAI con el id {request.CaiId} se encuentra inactivo");
            }
            if(!caiExist.IsGeneralCai && caiExist.BranchOfficeId != request.BranchOfficeId)
            {
                throw new ApiException($"El CAI {caiExist.Name} no pertenece a la sucursal {branchOfficeExist.Name}");
            }
            if(DateTime.UtcNow.Date > caiExist.ValidUntil)
            {
                throw new ApiException($"El CAI {caiExist.Name} se encuentra vencido por favor solicitar mas facturas");
            }
            if(caiExist.AvailableInvoices == 0)
            {
                throw new ApiException($"Lo sentimos no tiene facturas disponibles con este CAI");
            }
            if(request.SagCode != null && (request.ExemptedRegistrationCertificateNumber == null || request.ExemptOrderNumber == null))
            {
                throw new ApiException($"Se requiere un registro de exoneracion completo: Codigo SAG, Numero de Orden Exenta, y Nº de Contancia de Registro Exonerado.");
            }
            if (request.ExemptedRegistrationCertificateNumber != null && (request.SagCode == null || request.ExemptOrderNumber == null))
            {
                throw new ApiException($"Se requiere un registro de exoneracion completo: Codigo SAG, Numero de Orden Exenta, y Nº de Contancia de Registro Exonerado.");
            }
            if (request.ExemptOrderNumber != null && (request.SagCode == null || request.ExemptedRegistrationCertificateNumber == null))
            {
                throw new ApiException($"Se requiere un registro de exoneracion completo: Codigo SAG, Numero de Orden Exenta, y Nº de Contancia de Registro Exonerado.");
            }
            var taxesRates = await _taxRepositoryAsync.ListAsync();
            if (request.ProductsToSell != null)
            {
                var checkProducts = await CheckMasterDataOfProductsToSell(request.ProductsToSell, taxesRates);
                if (checkProducts != "true")
                {
                    throw new ApiException($"{checkProducts}");
                }
                var checkDescriptios = CheckDescriptionsAndNamesOfProductsToSell(request.ProductsToSell);
                if (checkDescriptios != "true")
                {
                    throw new ApiException($"{checkDescriptios}");
                }
            }
            var newRecord = _mapper.Map<Invoice>(request);
            newRecord.InvoiceNumber = CreateInvoiceNumber(caiExist);
            newRecord.TaxedAt15Percent = 0;
            newRecord.TaxedAt18Percent = 0;
            newRecord.Taxes15Percent = 0;
            newRecord.Taxes18Percent = 0;
            newRecord.Exempt = 0;
            newRecord.Exonerated = 0;
            newRecord.Outstanding = 0;
            var productsSold = new List<ProductSoldDto>();
            var invoiceResponse = await _repositoryAsync.AddAsync(newRecord);
            await _repositoryAsync.SaveChangesAsync();
            if(caiExist.AvailableInvoices < (caiExist.EndCorrelative - caiExist.StartCorrelative))
            {
                caiExist.AvailableInvoices = caiExist.AvailableInvoices - 1;
                caiExist.CurrentCorrelative = caiExist.CurrentCorrelative + 1;
            }
            else
            {
                caiExist.AvailableInvoices = caiExist.AvailableInvoices - 1;
            }
            await _caiRepositoryAsync.UpdateAsync(caiExist);
            await _caiRepositoryAsync.SaveChangesAsync();
            
            if (request.ProductsToSell != null && request.ProductsToSell.Count > 0)
            {
                foreach (var productToSell in request.ProductsToSell)
                {
                    var newProductToSell = _mapper.Map<ProductSold>(productToSell);
                    newProductToSell.InvoiceId = invoiceResponse.Id;
                    newProductToSell.UnitPrice = productToSell.RecomendedSalePrice;
                    newProductToSell.Taxes = TaxCalculator(productToSell, taxesRates);
                    newProductToSell.TotalLine = newProductToSell.Taxes + (newProductToSell.Quantity * productToSell.RecomendedSalePrice);
                    var productToSellResponse = await _productSoldRepositoryAsync.AddAsync(newProductToSell);
                    await _productSoldRepositoryAsync.SaveChangesAsync();
                    var newProductToSellDto = _mapper.Map<ProductSoldDto>(productToSellResponse);
                    productsSold.Add(newProductToSellDto);
                }
                
                newRecord.Exempt = CalculateGravableValue(productsSold, taxesRates.Find(x => x.Rate == 0));
                if(request.SagCode == null)
                {
                    newRecord.TaxedAt15Percent = CalculateGravableValue(productsSold, taxesRates.Find(x => x.Rate == 15));
                    newRecord.TaxedAt18Percent = CalculateGravableValue(productsSold, taxesRates.Find(x => x.Rate == 18));
                    newRecord.Taxes15Percent = CalculateTaxesValue(productsSold, taxesRates.Find(x => x.Rate == 15));
                    newRecord.Taxes18Percent = CalculateTaxesValue(productsSold, taxesRates.Find(x => x.Rate == 18));
                    newRecord.Exonerated = 0;
                    newRecord.Total = newRecord.TaxedAt15Percent + newRecord.TaxedAt18Percent + newRecord.Taxes15Percent + newRecord.Taxes18Percent + newRecord.Exempt;
                    newRecord.Outstanding = newRecord.Total;
                }
                else
                {
                    newRecord.Exonerated = CalculateTaxesValue(productsSold, taxesRates.Find(x => x.Rate == 15)) + CalculateTaxesValue(productsSold, taxesRates.Find(x => x.Rate == 18));

                }
                await _repositoryAsync.UpdateAsync(newRecord);
                await _repositoryAsync.SaveChangesAsync();
                invoiceResponse.TaxedAt15Percent = newRecord.TaxedAt15Percent;
                invoiceResponse.Taxes15Percent = newRecord.Taxes15Percent;
                invoiceResponse.TaxedAt18Percent = newRecord.TaxedAt18Percent;
                invoiceResponse.Taxes18Percent = newRecord.Taxes18Percent;
                invoiceResponse.Exempt = newRecord.Exempt;
                invoiceResponse.Exonerated = newRecord.Exonerated;
                invoiceResponse.Total = newRecord.Total;
                invoiceResponse.Outstanding = newRecord.Outstanding;
            }
            var productsForNewInvoice = await _productSoldRepositoryAsync.ListAsync(new ProductSoldSpecification(invoiceResponse.Id));
            var productsDto = _mapper.Map<List<ProductSoldDto>>(productsForNewInvoice);
            var dto = _mapper.Map<InvoiceDto>(invoiceResponse);
            dto.ProductsSold = productsDto;
            return new Response<InvoiceDto>(dto, $"Factura {dto.InvoiceNumber} creada exitosamente.");
        }
        static public string CheckDescriptionsAndNamesOfProductsToSell(List<ProductToSellDto> request)
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
        public async Task<string> CheckMasterDataOfProductsToSell(List<ProductToSellDto> request, List<Tax> taxes)
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
        public static string CreateInvoiceNumber(Cai cai)
        {
            var numberOfCharacters = cai.Prefix.ToCharArray().Length;
            var numberOfCharactersInId = cai.CurrentCorrelative.ToString().ToCharArray().Length;
            var code = "";
            if(cai.CurrentCorrelative != cai.StartCorrelative)
            {
                if (numberOfCharacters + numberOfCharactersInId < 19)
                {
                    int characterOffset = 19 - (numberOfCharacters + numberOfCharactersInId);
                    code = cai.Prefix + new string('0', characterOffset) + (cai.CurrentCorrelative + 1).ToString();
                }
                else
                {
                    code = cai.Prefix + (cai.CurrentCorrelative + 1).ToString();
                }
                return code;
            }
            else
            {
                if(cai.AvailableInvoices < cai.EndCorrelative - cai.StartCorrelative)
                {
                    if (numberOfCharacters + numberOfCharactersInId < 19)
                    {
                        int characterOffset = 19 - (numberOfCharacters + numberOfCharactersInId);
                        code = cai.Prefix + new string('0', characterOffset) + (cai.CurrentCorrelative + 1).ToString();
                    }
                    else
                    {
                        code = cai.Prefix + (cai.CurrentCorrelative + 1).ToString();
                    }
                    return code;
                }
                else
                {
                    if (numberOfCharacters + numberOfCharactersInId < 19)
                    {
                        int characterOffset = 19 - (numberOfCharacters + numberOfCharactersInId);
                        code = cai.Prefix + new string('0', characterOffset) + cai.CurrentCorrelative.ToString();
                    }
                    else
                    {
                        code = cai.Prefix + cai.CurrentCorrelative.ToString();
                    }
                    return code;
                }
               
              
            }
            
        }
        static public decimal TaxCalculator(ProductToSellDto product, List<Tax> taxes)
        {
            Tax productTax = null;
            productTax = taxes.Find(x => x.Id == product.TaxId);
            decimal gravable = product.Quantity * product.RecomendedSalePrice;
            decimal total = gravable * ((productTax.Rate / 100) + 1);
            decimal tax = total - gravable;
            return tax;
        }
        static public decimal CalculateGravableValue(List<ProductSoldDto> products, Tax tax)
        {
            decimal gravable = 0;
            foreach (var product in products)
            {
                if (product.TaxId == tax.Id)
                {
                    gravable += (product.Quantity * product.UnitPrice);
                }
            }
            return gravable;
        }
        public static decimal CalculateTaxesValue(List<ProductSoldDto> products, Tax tax)
        {
            decimal taxes = 0;
            foreach (var product in products)
            {
                if (product.TaxId == tax.Id)
                {
                    decimal taxAmount = (product.Quantity * product.UnitPrice * (tax.Rate / 100));
                    taxes += taxAmount;
                }
            }
            return taxes;
        }

    }
}
