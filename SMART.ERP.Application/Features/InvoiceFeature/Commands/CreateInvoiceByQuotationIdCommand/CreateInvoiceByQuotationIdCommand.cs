using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Invoice;
using SMART.ERP.Application.DTOs.Quotation;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.AccountingPostingService;
using SMART.ERP.Application.Services.InventoryMovementService;
using SMART.ERP.Application.Services.JwtService;
using SMART.ERP.Application.Services.ProductCacheInvalidator;
using SMART.ERP.Application.Services.ProductCompositionService;
using SMART.ERP.Application.Specifications.ProductSoldSpecification;
using SMART.ERP.Application.Specifications.QuotationSpecification;
using SMART.ERP.Application.Specifications.WarehouseSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Domain.Enums;

namespace SMART.ERP.Application.Features.InvoiceFeature.Commands.CreateInvoiceByQuotationIdCommand
{
    public class CreateInvoiceByQuotationIdCommand : IRequest<Response<string>>
    {
        public int QuotationId { get; set; }
        public int CaiId { get; set; }
        public int BranchOfficeId { get; set; }
        public string? PurchaseOrderCode { get; set; }
        public string? SagCode { get; set; }
        public string? ExemptOrderNumber { get; set; }
        public string? ExemptedRegistrationCertificateNumber { get; set; }
        public int InvoicePaymentTypeId { get; set; }
        public DateOnly? ExpectedPaymentDate { get; set; }
        /// <summary>Marca la factura como suscripción prepagada (SaaS): el ingreso va a Ingresos Diferidos y se devenga 1/N mensual.</summary>
        public bool IsDeferredRevenue { get; set; }
        /// <summary>Meses sobre los que se devenga el ingreso (típico 12 para anual). Requerido si IsDeferredRevenue=true.</summary>
        public int? RecognitionMonths { get; set; }
    }
    public class CreateInvoiceByQuotationIdCommandHandler : IRequestHandler<CreateInvoiceByQuotationIdCommand, Response<string>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<Invoice> _repositoryAsync;
        private readonly IRepositoryAsync<Cai> _caiRepositoryAsync;
        private readonly IRepositoryAsync<Tax> _taxRepositoryAsync;
        private readonly IRepositoryAsync<Product> _productRepositoryAsync;
        private readonly IRepositoryAsync<ProductSold> _productSoldRepositoryAsync;
        private readonly IRepositoryAsync<Quotation> _quotationRepositoryAsync;
        private readonly IRepositoryAsync<Warehouse> _warehouseRepositoryAsync;
        private readonly IRepositoryAsync<InvoicePaymentType> _invoicePaymentTypeRepositoryAsync;
        private readonly IJwtService _jwtService;
        private readonly IInventoryMovementService _movementService;
        private readonly IProductCompositionService _compositionService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IProductCacheInvalidator _cacheInvalidator;
        private readonly IAccountingPostingService _accountingPostingService;
        public CreateInvoiceByQuotationIdCommandHandler(IMapper mapper, IJwtService jwtService, IRepositoryAsync<InvoicePaymentType> invoicePaymentTypeRepositoryAsync, IRepositoryAsync<Warehouse> warehouseRepositoryAsync, IRepositoryAsync<Invoice> repositoryAsync, IRepositoryAsync<Cai> caiRepositoryAsync, IRepositoryAsync<Tax> taxRepositoryAsync, IRepositoryAsync<Product> productRepositoryAsync, IRepositoryAsync<ProductSold> productSoldRepositoryAsync, IRepositoryAsync<Quotation> quotationRepositoryAsync, IInventoryMovementService movementService, IProductCompositionService compositionService, IUnitOfWork unitOfWork, IProductCacheInvalidator cacheInvalidator, IAccountingPostingService accountingPostingService)
        {
            _accountingPostingService = accountingPostingService;
            _mapper = mapper;
            _repositoryAsync = repositoryAsync;
            _caiRepositoryAsync = caiRepositoryAsync;
            _taxRepositoryAsync = taxRepositoryAsync;
            _productRepositoryAsync = productRepositoryAsync;
            _productSoldRepositoryAsync = productSoldRepositoryAsync;
            _quotationRepositoryAsync = quotationRepositoryAsync;
            _warehouseRepositoryAsync = warehouseRepositoryAsync;
            _invoicePaymentTypeRepositoryAsync = invoicePaymentTypeRepositoryAsync;
            _jwtService = jwtService;
            _movementService = movementService;
            _compositionService = compositionService;
            _unitOfWork = unitOfWork;
            _cacheInvalidator = cacheInvalidator;
        }
        public async Task<Response<string>> Handle(CreateInvoiceByQuotationIdCommand request, CancellationToken cancellationToken)
        {
            var quotationExist = await _quotationRepositoryAsync.FirstOrDefaultAsync(new FilterQuotationByIdSpecification(request.QuotationId));
            if (quotationExist == null)
            {
                throw new ApiException($"No existe una cotizacion con el id {request.QuotationId}");
            }
            var customerName = quotationExist.Customer?.FullName;
            var caiExist = await _caiRepositoryAsync.GetByIdAsync(request.CaiId);
            if (caiExist == null)
            {
                throw new ApiException($"No existe un CAI con el id {request.CaiId}");
            }
            var invoicePaymentTypeExist = await _invoicePaymentTypeRepositoryAsync.GetByIdAsync(request.InvoicePaymentTypeId);
            if (invoicePaymentTypeExist == null)
            {
                throw new ApiException($"No existe un tipo de pago con el id {request.InvoicePaymentTypeId}");
            }
            if (!caiExist.IsActive)
            {
                throw new ApiException($"El CAI con el id {request.CaiId} se encuentra inactivo");
            }
            if (!caiExist.IsGeneralCai && caiExist.BranchOfficeId != request.BranchOfficeId)
            {
                throw new ApiException($"El CAI {caiExist.Name} no pertenece a la sucursal {quotationExist.BranchOffice!.Name}");
            }
            if (DateTime.UtcNow.Date > caiExist.ValidUntil)
            {
                throw new ApiException($"El CAI {caiExist.Name} se encuentra vencido por favor solicitar mas facturas");
            }
            if (caiExist.AvailableInvoices == 0)
            {
                throw new ApiException($"Lo sentimos no tiene facturas disponibles con este CAI");
            }
            if (request.SagCode != null && (request.ExemptedRegistrationCertificateNumber == null || request.ExemptOrderNumber == null))
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
            if (request.IsDeferredRevenue && (request.RecognitionMonths == null || request.RecognitionMonths <= 0))
            {
                throw new ApiException("Las facturas marcadas como suscripción prepagada requieren un número de meses de devengo mayor a cero.");
            }
            var taxesRates = await _taxRepositoryAsync.ListAsync();
            int createdInvoiceId = 0;
            await _unitOfWork.ExecuteInTransactionAsync(async ct =>
            {
                var newRecord = new Invoice { CustomerId = quotationExist.CustomerId, CaiId = request.CaiId, BranchOfficeId = quotationExist.BranchOfficeId, UserId = quotationExist.UserId, CreationDate = DateTime.Now, Observations = quotationExist.Observations, TermsAndConditions = quotationExist.TermsAndConditions, StatusId = 16, PurchaseOrderCode = request.PurchaseOrderCode, SagCode = request.SagCode, ExemptOrderNumber = request.ExemptOrderNumber, ExemptedRegistrationCertificateNumber = request.ExemptedRegistrationCertificateNumber, QuotationOriginId = request.QuotationId, InvoicePaymentTypeId = request.InvoicePaymentTypeId };

                newRecord.InvoiceNumber = CreateInvoiceNumber(caiExist);
                newRecord.TaxedAt15Percent = 0;
                newRecord.TaxedAt18Percent = 0;
                newRecord.Taxes15Percent = 0;
                newRecord.Taxes18Percent = 0;
                newRecord.Exempt = 0;
                newRecord.Exonerated = 0;
                newRecord.Outstanding = 0;
                newRecord.CreatedBy = _jwtService.GetSubjectToken();
                newRecord.InsertedDate = DateTime.UtcNow;
                newRecord.IsDeferredRevenue = request.IsDeferredRevenue;
                if (request.IsDeferredRevenue)
                {
                    newRecord.RecognitionMonths = request.RecognitionMonths;
                    newRecord.RecognitionStartDate = newRecord.CreationDate;
                    newRecord.MonthsRecognized = 0;
                    newRecord.NextRecognitionDate = newRecord.CreationDate.AddMonths(1);
                }
                var productsSold = new List<ProductSoldDto>();
                var invoiceResponse = await _repositoryAsync.AddAsync(newRecord);
                await _repositoryAsync.SaveChangesAsync();
                if (caiExist.AvailableInvoices < (caiExist.EndCorrelative - caiExist.StartCorrelative))
                {
                    caiExist.AvailableInvoices--;
                    caiExist.CurrentCorrelative++;
                }
                else
                {
                    caiExist.AvailableInvoices--;
                }
                await _caiRepositoryAsync.UpdateAsync(caiExist);
                await _caiRepositoryAsync.SaveChangesAsync();
                quotationExist.Prefix = null;
                quotationExist.BranchOffice = null;
                quotationExist.User = null;
                quotationExist.Customer = null;
                quotationExist.Status = null;
                for (int i = 0; i < quotationExist?.ProductsOffered?.Count; i++)
                {
                    quotationExist.ProductsOffered[i].Tax = null;
                    quotationExist.ProductsOffered[i].Product!.Brand = null;
                }
                quotationExist!.StatusId = 7;
                quotationExist.InvoiceDestinationId = invoiceResponse.Id;
                var productsOffered = quotationExist.ProductsOffered;
                quotationExist.ProductsOffered = null;
                await _quotationRepositoryAsync.UpdateAsync(quotationExist);
                await _quotationRepositoryAsync.SaveChangesAsync();
                quotationExist.ProductsOffered = productsOffered;
                var quotationExistDto = _mapper.Map<QuotationDto>(quotationExist);
                if (quotationExistDto.ProductsOffered != null && quotationExistDto.ProductsOffered.Count > 0)
                {
                    foreach (var productToSell in quotationExistDto.ProductsOffered)
                    {
                        var newProductToSell = new ProductSold
                        {
                            InvoiceId = invoiceResponse.Id,
                            ProductId = productToSell.ProductId,
                            ProductCode = productToSell.ProductCode,
                            ProductDescription = productToSell.ProductDescription,
                            UnitPrice = productToSell.UnitPrice,
                            Quantity = productToSell.Quantity,
                            TaxId = productToSell.TaxId,
                            Taxes = TaxCalculator(productToSell, taxesRates),
                            TotalLine = TaxCalculator(productToSell, taxesRates) + (productToSell.Quantity * productToSell.UnitPrice)
                        };

                        var productToSellResponse = await _productSoldRepositoryAsync.AddAsync(newProductToSell);
                        await _productSoldRepositoryAsync.SaveChangesAsync();
                        var newProductToSellDto = _mapper.Map<ProductSoldDto>(productToSellResponse);
                        productsSold.Add(newProductToSellDto);
                    }

                    newRecord.Exempt = CalculateGravableValue(productsSold, taxesRates.Find(x => x.Rate == 0));
                    if (request.SagCode == null)
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
                await RegisterKardexSaleMovements(productsForNewInvoice, invoiceResponse.BranchOfficeId, invoiceResponse, customerName, KardexMovementType.Sale, ct);
                await _accountingPostingService.PostInvoiceAsync(invoiceResponse.Id, ct);
                createdInvoiceId = invoiceResponse.Id;
            }, cancellationToken);

            await _cacheInvalidator.InvalidateAsync(cancellationToken);
            return new Response<string>(createdInvoiceId.ToString());
        }
        /// <summary>
        /// Registra una salida de Kardex (venta) por cada producto de inventario de la factura.
        /// El servicio sincroniza InventoryDistribution y Product.CurrentStock. Las líneas sin
        /// ProductId (texto libre / servicios) no afectan inventario y se omiten. No valida stock:
        /// se conserva el comportamiento permisivo (la sobreventa deja saldos negativos).
        /// </summary>
        public async Task RegisterKardexSaleMovements(List<ProductSold> products, int branchOfficeId, Invoice invoice, string? thirdPartyName, KardexMovementType movementType, CancellationToken cancellationToken)
        {
            var warehouseExist = await _warehouseRepositoryAsync.FirstOrDefaultAsync(new FilterWarehouseByBranchOfficeIdSpecification(branchOfficeId));
            if (warehouseExist == null) { return; }

            var userId = _jwtService.GetUidToken();
            var userName = _jwtService.GetSubjectToken();
            var affectedProductIds = new List<int>();

            foreach (var product in products)
            {
                if (product.ProductId == null) { continue; }

                var rows = await _compositionService.ResolveComponentsForSaleAsync(product.ProductId.Value, product.Quantity, cancellationToken);
                if (rows.Count == 0) { continue; }

                foreach (var row in rows)
                {
                    await _movementService.RecordMovementAsync(new RecordMovementInput
                    {
                        ProductId = row.ComponentProductId,
                        WarehouseId = warehouseExist.Id,
                        MovementDate = invoice.CreationDate,
                        MovementType = movementType,
                        DocumentType = "Invoice",
                        DocumentId = invoice.Id,
                        DocumentCode = invoice.InvoiceNumber,
                        ThirdPartyName = thirdPartyName,
                        QuantityIn = 0m,
                        QuantityOut = row.QuantityOut,
                        UnitCost = row.UnitCost,
                        UserId = userId,
                        UserName = userName,
                        SyncStock = true,
                        UpdateProductCost = false
                    }, cancellationToken);

                    if (!affectedProductIds.Contains(row.ComponentProductId)) { affectedProductIds.Add(row.ComponentProductId); }
                }
            }

            foreach (var productId in affectedProductIds)
            {
                await _movementService.SyncProductStockAsync(productId, cancellationToken);
            }
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
        static public decimal TaxCalculator(ProductOfferedDto product, List<Tax> taxes)
        {
            Tax? productTax = null;
            productTax = taxes.Find(x => x.Id == product.TaxId);
            decimal gravable = product.Quantity * product.UnitPrice;
            decimal total = gravable * ((productTax!.Rate / 100) + 1);
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
    }
}
