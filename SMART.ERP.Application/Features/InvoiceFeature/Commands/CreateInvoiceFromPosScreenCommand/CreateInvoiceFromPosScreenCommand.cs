using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Invoice;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.JwtService;
using SMART.ERP.Application.Specifications.CustomerSpecification;
using SMART.ERP.Application.Specifications.ProductSoldSpecification;
using SMART.ERP.Application.Specifications.WarehouseSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.InvoiceFeature.Commands.CreateInvoiceFromPosScreenCommand
{
    public class CreateInvoiceFromPosScreenCommand : IRequest<Response<InvoiceDto>>
    {
        public Guid CustomerId { get; set; }
        public int CaiId { get; set; }
        public int BranchOfficeId { get; set; }
        public Guid UserId { get; set; }
        public DateTime CreationDate { get; set; }
        public List<ProductToSellDto>? ProductsToSell { get; set; }
        public int StatusId { get; set; }
        public int InvoicePaymentTypeId { get; set; }
        public List<PaymentBreakDownDto>? Payments { get; set; }
    }
    public class CreateInvoiceFromPosScreenCommandHandler : IRequestHandler<CreateInvoiceFromPosScreenCommand, Response<InvoiceDto>>
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
        private readonly IRepositoryAsync<Warehouse> _warehouseRepositoryAsync;
        private readonly IRepositoryAsync<InventoryDistribution> _inventoryDistributionRepositoryAsync;
        private readonly IRepositoryAsync<InvoicePaymentType> _invoicePaymentTypeRepositoryAsync;
        private readonly IRepositoryAsync<BillPayment> _billPaymentRepositoryAsync;
        private readonly IJwtService _jwtService;
        public CreateInvoiceFromPosScreenCommandHandler(IMapper mapper, IJwtService jwtService, IRepositoryAsync<Invoice> repositoryAsync, IRepositoryAsync<Cai> caiRepositoryAsync, IRepositoryAsync<Customer> customerRepositoryAsync, IRepositoryAsync<BranchOffices> branchOfficeRepositoryAsync, IRepositoryAsync<User> userRepositoryAsync, IRepositoryAsync<Status> statusRepositoryAsync, IRepositoryAsync<Tax> taxRepositoryAsync, IRepositoryAsync<Product> productRepositoryAsync, IRepositoryAsync<ProductSold> productSoldRepositoryAsync, IRepositoryAsync<Warehouse> warehouseRepositoryAsync, IRepositoryAsync<InventoryDistribution> inventoryDistributionRepositoryAsync, IRepositoryAsync<InvoicePaymentType> invoicePaymentTypeRepositoryAsync, IRepositoryAsync<BillPayment> billPaymentRepositoryAsync)
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
            _warehouseRepositoryAsync = warehouseRepositoryAsync;
            _inventoryDistributionRepositoryAsync = inventoryDistributionRepositoryAsync;
            _invoicePaymentTypeRepositoryAsync = invoicePaymentTypeRepositoryAsync;
            _billPaymentRepositoryAsync = billPaymentRepositoryAsync;
            _jwtService = jwtService;
        }


        public async Task<Response<InvoiceDto>> Handle(CreateInvoiceFromPosScreenCommand request, CancellationToken cancellationToken)
        {
            var customerExist = await _customerRepositoryAsync.FirstOrDefaultAsync(new FilterCustomerByMasterIdSpecification(request.CustomerId));
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
            if (!caiExist.IsActive)
            {
                throw new ApiException($"El CAI con el id {request.CaiId} se encuentra inactivo");
            }
            if (!caiExist.IsGeneralCai && caiExist.BranchOfficeId != request.BranchOfficeId)
            {
                throw new ApiException($"El CAI {caiExist.Name} no pertenece a la sucursal {branchOfficeExist.Name}");
            }
            if (DateTime.UtcNow.Date > caiExist.ValidUntil)
            {
                throw new ApiException($"El CAI {caiExist.Name} se encuentra vencido por favor solicitar mas facturas");
            }
            if (caiExist.AvailableInvoices == 0)
            {
                throw new ApiException($"Lo sentimos no tiene facturas disponibles con este CAI");
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
            if (caiExist.AvailableInvoices < (caiExist.EndCorrelative - caiExist.StartCorrelative))
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
                newRecord.TaxedAt15Percent = CalculateGravableValue(productsSold, taxesRates.Find(x => x.Rate == 15));
                newRecord.TaxedAt18Percent = CalculateGravableValue(productsSold, taxesRates.Find(x => x.Rate == 18));
                newRecord.Taxes15Percent = CalculateTaxesValue(productsSold, taxesRates.Find(x => x.Rate == 15));
                newRecord.Taxes18Percent = CalculateTaxesValue(productsSold, taxesRates.Find(x => x.Rate == 18));
                newRecord.Exonerated = 0;
                newRecord.Total = newRecord.TaxedAt15Percent + newRecord.TaxedAt18Percent + newRecord.Taxes15Percent + newRecord.Taxes18Percent + newRecord.Exempt;
                newRecord.Outstanding = newRecord.Total;

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
            if (request.Payments.Count > 0)
            {
                var invoicePayments = new List<BillPayment>();
                foreach (var item in request.Payments)
                {
                    var newBillPayment = new BillPayment
                    {
                        InvoiceId = invoiceResponse.Id,
                        TypeOfPaymentMethodId = item.TypeOfPaymentMethodId,
                        Date = invoiceResponse.CreationDate,
                        Amount = item.Amount,
                        CreationDate = DateTime.UtcNow,
                        CreatedBy = _jwtService.GetSubjectToken(),
                    };
                    invoicePayments.Add(newBillPayment);
                    await _billPaymentRepositoryAsync.AddAsync(newBillPayment);
                }
                await _billPaymentRepositoryAsync.SaveChangesAsync();
                invoiceResponse.BillPayments = invoicePayments;
            }
            invoiceResponse.Status = statusExist;
            invoiceResponse.Customer = customerExist;
            var productsForNewInvoice = await _productSoldRepositoryAsync.ListAsync(new ProductSoldSpecification(invoiceResponse.Id));
            var productsDto = _mapper.Map<List<ProductSoldDto>>(productsForNewInvoice);
            var dto = _mapper.Map<InvoiceDto>(invoiceResponse);
            dto.ProductsSold = productsDto;
            await removeProductsFromWarehouses(productsForNewInvoice, request.BranchOfficeId);
            return new Response<InvoiceDto>(dto, $"Factura {dto.InvoiceNumber} creada exitosamente.");

        }
        public async Task removeProductsFromWarehouses(List<ProductSold> products, int branchOfficeId)
        {
            var warehouseExist = await _warehouseRepositoryAsync.FirstOrDefaultAsync(new FilterWarehouseByBranchOfficeIdSpecification(branchOfficeId));
            if (warehouseExist != null)
            {
                foreach (var product in products)
                {
                    var productExist = warehouseExist.InventoryDistributions.FirstOrDefault(x => x.ProductId == product.ProductId);
                    if (productExist != null)
                    {
                        productExist.Quantity = productExist.Quantity - product.Quantity;
                        await _inventoryDistributionRepositoryAsync.UpdateAsync(productExist);
                    }
                    else
                    {
                        var newInventoryDistribution = new InventoryDistribution
                        {
                            ProductId = product.ProductId.Value,
                            WarehouseId = warehouseExist.Id,
                            Quantity = 0 - product.Quantity
                        };
                        await _inventoryDistributionRepositoryAsync.AddAsync(newInventoryDistribution);
                    }
                }
                await _inventoryDistributionRepositoryAsync.SaveChangesAsync();
            }
            foreach (var product in products)
            {
                var productExist = await _productRepositoryAsync.GetByIdAsync(product.ProductId.Value);
                productExist.CurrentStock -= (int)product.Quantity;
                await _productRepositoryAsync.UpdateAsync(productExist);
            }
            await _productRepositoryAsync.SaveChangesAsync();
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
        public static string CreateInvoiceNumber(Cai cai)
        {
            var numberOfCharacters = cai.Prefix.ToCharArray().Length;
            var numberOfCharactersInId = cai.CurrentCorrelative.ToString().ToCharArray().Length;
            var code = "";
            if (cai.CurrentCorrelative != cai.StartCorrelative)
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
                if (cai.AvailableInvoices < cai.EndCorrelative - cai.StartCorrelative)
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
        static public string CheckDescriptionsAndNamesOfProductsToSell(List<ProductToSellDto> request)
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
    }
}
