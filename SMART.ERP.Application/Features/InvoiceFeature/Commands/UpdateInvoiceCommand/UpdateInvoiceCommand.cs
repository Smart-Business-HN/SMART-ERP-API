using AutoMapper;
using MediatR;
using Newtonsoft.Json;
using SMART.ERP.Application.DTOs.Invoice;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.JwtService;
using SMART.ERP.Application.Specifications.InvoiceSpecification;
using SMART.ERP.Application.Specifications.ProductSoldSpecification;
using SMART.ERP.Application.Specifications.WarehouseSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.InvoiceFeature.Commands.UpdateInvoiceCommand
{
    public class UpdateInvoiceCommand : IRequest<Response<InvoiceDto>>
    {
        public int Id { get; set; }
        public Guid CustomerId { get; set; }
        public int BranchOfficeId { get; set; }
        public Guid? UserId { get; set; }
        public DateTime CreationDate { get; set; }
        public string? Observations { get; set; }
        public string? TermsAndConditions { get; set; }
        public List<ProductToSellDto>? ProductsToSell { get; set; }
        public List<ProductSoldDto>? ProductsSold { get; set; }
        public int StatusId { get; set; }
        public string? PurchaseOrderCode { get; set; }
        public string? SagCode { get; set; }
        public string? ExemptOrderNumber { get; set; }
        public string? ExemptedRegistrationCertificateNumber { get; set; }
        public int InvoicePaymentTypeId { get; set; }
        public DateOnly? ExpectedPaymentDate { get; set; }
        public int? ProjectId { get; set; }
    }
    public class UpdateInvoiceCommandHandler : IRequestHandler<UpdateInvoiceCommand, Response<InvoiceDto>>
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
        private readonly IJwtService _jwtService;
        public UpdateInvoiceCommandHandler(IMapper mapper, IJwtService jwtService,IRepositoryAsync<Invoice> repositoryAsync, IRepositoryAsync<Cai> caiRepositoryAsync, IRepositoryAsync<Customer> customerRepositoryAsync, IRepositoryAsync<BranchOffices> branchOfficeRepositoryAsync, IRepositoryAsync<User> userRepositoryAsync, IRepositoryAsync<Status> statusRepositoryAsync, IRepositoryAsync<Tax> taxRepositoryAsync, IRepositoryAsync<Product> productRepositoryAsync, IRepositoryAsync<ProductSold> productSoldRepositoryAsync, IRepositoryAsync<Warehouse> warehouseRepositoryAsync, IRepositoryAsync<InventoryDistribution> inventoryDistributionRepositoryAsync)
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
            _jwtService = jwtService;
        }
        public async Task<Response<InvoiceDto>> Handle(UpdateInvoiceCommand request, CancellationToken cancellationToken)
        {
            var invoiceExist = await _repositoryAsync.FirstOrDefaultAsync(new FilterInvoiceByIdSpecification(request.Id));
            if (invoiceExist == null)
            {
                throw new ApiException($"No existe una factura con el Id {request.CustomerId}");
            }
            if (invoiceExist!.Status!.Name == "Gravada")
            {
                throw new ApiException($"La factura: {invoiceExist.InvoiceNumber} se encuentra cobrada y presentada al SAR.");
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
            //UPDATING VALUES
            if (invoiceExist.CustomerId != request.CustomerId)
            {
                invoiceExist.CustomerId = request.CustomerId;
                invoiceExist.Customer = null;
            }
            if (invoiceExist.BranchOfficeId != request.BranchOfficeId)
            {
                invoiceExist.BranchOfficeId = request.BranchOfficeId;
            }
            if (invoiceExist.StatusId != request.StatusId)
            {
                invoiceExist.StatusId = request.StatusId;
            }
            if (invoiceExist.Observations != request.Observations)
            {
                invoiceExist.Observations = request.Observations;
            }
            if (invoiceExist.TermsAndConditions != request.TermsAndConditions)
            {
                invoiceExist.TermsAndConditions = request.TermsAndConditions;
            }
            if (invoiceExist.PurchaseOrderCode != request.PurchaseOrderCode)
            {
                invoiceExist.PurchaseOrderCode = request.PurchaseOrderCode;
            }
            if (invoiceExist.CreationDate != request.CreationDate)
            {
                invoiceExist.CreationDate = request.CreationDate;
            }
            if (invoiceExist.SagCode != request.SagCode)
            {
                invoiceExist.SagCode = request.SagCode;
            }
            if (invoiceExist.ExemptedRegistrationCertificateNumber != request.ExemptedRegistrationCertificateNumber)
            {
                invoiceExist.ExemptedRegistrationCertificateNumber = request.ExemptedRegistrationCertificateNumber;
            }
            if (invoiceExist.ExemptOrderNumber != request.ExemptOrderNumber)
            {
                invoiceExist.ExemptOrderNumber = request.ExemptOrderNumber;
            }
            if (invoiceExist.InvoicePaymentTypeId != request.InvoicePaymentTypeId)
            {
                invoiceExist.InvoicePaymentTypeId = request.InvoicePaymentTypeId;
                invoiceExist.InvoicePaymentType = null;
            }
            if (invoiceExist.ExpectedPaymentDate != request.ExpectedPaymentDate)
            {
                invoiceExist.ExpectedPaymentDate = request.ExpectedPaymentDate;
            }
            if (invoiceExist.ProjectId != request.ProjectId)
            {
                invoiceExist.ProjectId = request.ProjectId;
            }
            var taxesRates = await _taxRepositoryAsync.ListAsync();
            string localProductSoldJson = JsonConvert.SerializeObject(request.ProductsSold);
            var productsPreExistence = JsonConvert.DeserializeObject<List<ProductSoldDto>>(localProductSoldJson);
            string localProductsToSellJson = JsonConvert.SerializeObject(request.ProductsToSell);
            var productsToProcess = JsonConvert.DeserializeObject<List<ProductToSellDto>>(localProductsToSellJson);
            string localProductSoldJson1 = JsonConvert.SerializeObject(request.ProductsSold);
            var productsPreExistence1 = JsonConvert.DeserializeObject<List<ProductSoldDto>>(localProductSoldJson);
            string localProductsToSellJson1 = JsonConvert.SerializeObject(request.ProductsToSell);
            var productsToProcess1 = JsonConvert.DeserializeObject<List<ProductToSellDto>>(localProductsToSellJson);
            var productsSold = await CheckProducts(request.ProductsSold!, request.ProductsToSell!, request.Id, taxesRates);
            invoiceExist.Exempt = CalculateGravableValue(productsSold, taxesRates.Find(x => x.Rate == 0)!);
            if (request.SagCode == null)
            {
                invoiceExist.TaxedAt15Percent = CalculateGravableValue(productsSold, taxesRates.Find(x => x.Rate == 15)!);
                invoiceExist.TaxedAt18Percent = CalculateGravableValue(productsSold, taxesRates.Find(x => x.Rate == 18)!);
                invoiceExist.Taxes15Percent = CalculateTaxesValue(productsSold, taxesRates.Find(x => x.Rate == 15)!);
                invoiceExist.Taxes18Percent = CalculateTaxesValue(productsSold, taxesRates.Find(x => x.Rate == 18)!);
                invoiceExist.Exonerated = 0;
                invoiceExist.Total = invoiceExist.TaxedAt15Percent + invoiceExist.TaxedAt18Percent + invoiceExist.Taxes15Percent + invoiceExist.Taxes18Percent + invoiceExist.Exempt;
                invoiceExist.Outstanding = invoiceExist.Total - invoiceExist!.BillPayments!.Sum(x => x.Amount);
            }
            else
            {
                invoiceExist.Exonerated = CalculateGravableValue(productsSold, taxesRates.Find(x => x.Rate == 15)!) + CalculateGravableValue(productsSold, taxesRates.Find(x => x.Rate == 18)!);
                invoiceExist.TaxedAt15Percent = 0;
                invoiceExist.TaxedAt18Percent = 0;
                invoiceExist.Taxes15Percent = 0;
                invoiceExist.Taxes18Percent = 0;
                invoiceExist.Total = invoiceExist.Exonerated;
            }
            invoiceExist.BillPayments = null;
            invoiceExist.ProductsSold = null;
            invoiceExist.ModificatedBy = _jwtService.GetSubjectToken();
            invoiceExist.ModificationDate = DateTime.UtcNow;
            await _repositoryAsync.UpdateAsync(invoiceExist);
            await _repositoryAsync.SaveChangesAsync();
            await UpdateStock(productsPreExistence!, productsToProcess!, invoiceExist.BranchOfficeId);
            await UpdateMainStock(productsPreExistence1!, productsToProcess1!);
            invoiceExist.Customer = customerExist;
            var dto = _mapper.Map<InvoiceDto>(invoiceExist);
            dto.ProductsSold = productsSold;
            return new Response<InvoiceDto>(dto, $"Factura {invoiceExist.InvoiceNumber} actualizada exitosamente.");

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
        public async Task<List<ProductSoldDto>> CheckProducts(List<ProductSoldDto> productSold, List<ProductToSellDto> productsToSell, int invoiceId, List<Tax> taxesRates)
        {
            string localProductSoldJson = JsonConvert.SerializeObject(productSold);
            var productsPreExistence = JsonConvert.DeserializeObject<List<ProductSoldDto>>(localProductSoldJson);
            string localProductsToSellJson = JsonConvert.SerializeObject(productsToSell);
            var productsToProcess = JsonConvert.DeserializeObject<List<ProductToSellDto>>(localProductsToSellJson);

            // Actualizar productos existentes
            foreach (var productExisting in productSold)
            {
                var productToUpdate = productExisting;
                var matchingProduct = productsToProcess!.FirstOrDefault(p => p.ProductId == productExisting.ProductId);

                if (matchingProduct != null)
                {
                    bool needsUpdate = false;

                    if (productToUpdate.Quantity != matchingProduct.Quantity)
                    {
                        productToUpdate.Quantity = matchingProduct.Quantity;
                        needsUpdate = true;
                    }

                    if (productToUpdate.UnitPrice != matchingProduct.RecomendedSalePrice)
                    {
                        productToUpdate.UnitPrice = matchingProduct.RecomendedSalePrice;
                        needsUpdate = true;
                    }

                    if (productToUpdate.ProductDescription != matchingProduct.ProductDescription)
                    {
                        productToUpdate.ProductDescription = matchingProduct.ProductDescription;
                        needsUpdate = true;
                    }

                    if (needsUpdate)
                    {
                        productToUpdate.Taxes = TaxCalculator(matchingProduct, taxesRates);
                        productToUpdate.TotalLine = productToUpdate.Taxes + (productToUpdate.Quantity * productToUpdate.UnitPrice);
                        productToUpdate.Product = null;
                        productToUpdate.Invoice = null;
                        productToUpdate.Tax = null;

                        var productSeed = _mapper.Map<ProductSold>(productToUpdate);
                        await _productSoldRepositoryAsync.UpdateAsync(productSeed);
                        await _productSoldRepositoryAsync.SaveChangesAsync();
                    }
                    var productToRemoveFromPreExistenceArray = productsPreExistence!.FirstOrDefault(x => x.ProductId == productToUpdate.ProductId);
                    productsPreExistence!.Remove(productToRemoveFromPreExistenceArray!);
                    productsToProcess!.Remove(matchingProduct);
                }
            }

            // Añadir nuevos productos a la factura
            foreach (var newProductToSell in productsToProcess!)
            {
                var newProductOffered = _mapper.Map<ProductSold>(newProductToSell);
                newProductOffered.InvoiceId = invoiceId;
                newProductOffered.UnitPrice = newProductToSell.RecomendedSalePrice;
                newProductOffered.Taxes = TaxCalculator(newProductToSell, taxesRates);
                newProductOffered.TotalLine = newProductToSell.Quantity * newProductToSell.RecomendedSalePrice;
                await _productSoldRepositoryAsync.AddAsync(newProductOffered);
                await _productSoldRepositoryAsync.SaveChangesAsync();
            }

            // Eliminar productos anteriores de la factura
            foreach (var productPreExistence in productsPreExistence!)
            {
                productPreExistence.Tax = null;
                productPreExistence.Invoice = null;
                productPreExistence.Product = null;
                var dtoToDelete = _mapper.Map<ProductSold>(productPreExistence);
                await _productSoldRepositoryAsync.DeleteAsync(dtoToDelete);
                await _productSoldRepositoryAsync.SaveChangesAsync();
            }

            // Obtener nuevos productos de la factura
            var newProducts = await _productSoldRepositoryAsync.ListAsync(new ProductSoldSpecification(invoiceId));
            var dtos = _mapper.Map<List<ProductSoldDto>>(newProducts);

            return dtos;
        }
        static public decimal TaxCalculator(ProductToSellDto product, List<Tax> taxes)
        {
            Tax productTax = taxes.Find(x => x.Id == product.TaxId)!;
            decimal gravable = product.Quantity * product.RecomendedSalePrice;
            decimal total = gravable * ((productTax!.Rate / 100) + 1);
            decimal tax = total - gravable;
            return tax;
        }
        public async Task UpdateStock(List<ProductSoldDto> productsSold, List<ProductToSellDto> productsToSell, int branchOfficeId)
        {
            var warehouse = await _warehouseRepositoryAsync.FirstOrDefaultAsync(new FilterWarehouseByBranchOfficeIdSpecification(branchOfficeId));
            if (warehouse == null) { return; }
            var productsToProcess = new List<ProductToSellDto>(productsToSell);
            var productsPreExistence = new List<ProductSoldDto>(productsSold);
            //Modificar los Productos existentes
            foreach (var productExisting in productsSold)
            {
                var productToUpdate = productExisting;
                var matchingProduct = productsToSell.FirstOrDefault(p => p.ProductId == productToUpdate.ProductId);
                if(matchingProduct == null)
                {
                    continue;
                }
                if(matchingProduct.Quantity !=  productToUpdate.Quantity)
                {
                    if(matchingProduct.Quantity > productToUpdate.Quantity)
                    {
                        var difference = productToUpdate.Quantity - matchingProduct.Quantity;
                        var currentStock = warehouse.InventoryDistributions!.FirstOrDefault(p => p.ProductId == productToUpdate.ProductId);
                        if(currentStock == null)
                        {
                            var newInventoryDistribution = new InventoryDistribution();
                            newInventoryDistribution.WarehouseId = warehouse.Id;
                            newInventoryDistribution.ProductId = productToUpdate.Id;
                            newInventoryDistribution.Quantity = matchingProduct.Quantity;
                            await _inventoryDistributionRepositoryAsync.AddAsync(newInventoryDistribution);
                        }
                        else
                        {
                            currentStock.Quantity += difference;
                            await _inventoryDistributionRepositoryAsync.UpdateAsync(currentStock);
                        }
                        await _inventoryDistributionRepositoryAsync.SaveChangesAsync();
                    }
                    else
                    {
                        var difference = productToUpdate.Quantity - matchingProduct.Quantity;
                        var currentStock = warehouse.InventoryDistributions!.FirstOrDefault(p => p.ProductId == productToUpdate.ProductId);
                        if (currentStock == null)
                        {
                            var newInventoryDistribution = new InventoryDistribution();
                            newInventoryDistribution.WarehouseId = warehouse.Id;
                            newInventoryDistribution.ProductId = productToUpdate.Id;
                            newInventoryDistribution.Quantity = matchingProduct.Quantity;
                            await _inventoryDistributionRepositoryAsync.AddAsync(newInventoryDistribution);
                        }
                        else
                        {
                            currentStock.Quantity += difference;
                            await _inventoryDistributionRepositoryAsync.UpdateAsync(currentStock);
                        }
                        await _inventoryDistributionRepositoryAsync.SaveChangesAsync();
                    }
                }
                productsPreExistence.Remove(productExisting);
                productsToProcess.Remove(matchingProduct);
            }
            //Remover stock de productos nuevos en la factura
            foreach (var newProductToSell in productsToProcess)
            {
                var currentStock = warehouse.InventoryDistributions!.FirstOrDefault(p => p.ProductId == newProductToSell.ProductId);
                if (currentStock == null)
                {
                    var newInventoryDistribution = new InventoryDistribution();
                    newInventoryDistribution.WarehouseId = warehouse.Id;
                    newInventoryDistribution.ProductId = newProductToSell.ProductId!.Value;
                    newInventoryDistribution.Quantity = 0 - newProductToSell.Quantity;
                    await _inventoryDistributionRepositoryAsync.AddAsync(newInventoryDistribution);
                }
                else
                {
                    currentStock.Quantity -= newProductToSell.Quantity;
                    await _inventoryDistributionRepositoryAsync.UpdateAsync(currentStock);
                }
                await _inventoryDistributionRepositoryAsync.SaveChangesAsync();
                
            }
            //Devolver Stock de productos eliminados de la factura
            foreach (var productPreExistence in productsPreExistence)
            {
                var currentStock = warehouse.InventoryDistributions!.FirstOrDefault(p => p.ProductId == productPreExistence.ProductId);
                if (currentStock == null)
                {
                    var newInventoryDistribution = new InventoryDistribution();
                    newInventoryDistribution.WarehouseId = warehouse.Id;
                    newInventoryDistribution.ProductId = productPreExistence.ProductId!.Value;
                    newInventoryDistribution.Quantity = productPreExistence.Quantity;
                    await _inventoryDistributionRepositoryAsync.AddAsync(newInventoryDistribution);
                }
                else
                {
                    currentStock.Quantity += productPreExistence.Quantity;
                    await _inventoryDistributionRepositoryAsync.UpdateAsync(currentStock);
                }
                await _inventoryDistributionRepositoryAsync.SaveChangesAsync();
            }
        }
        public async Task UpdateMainStock(List<ProductSoldDto> productsSold, List<ProductToSellDto> productsToSell)
        {
            var products = await _productRepositoryAsync.ListAsync();

            var productsToProcess = new List<ProductToSellDto>(productsToSell);
            var productsPreExistence = new List<ProductSoldDto>(productsSold);
            //Modificar los Productos existentes
            foreach (var productExisting in productsSold)
            {
                var productToUpdate = productExisting;
                var matchingProduct = productsToSell.FirstOrDefault(p => p.ProductId == productToUpdate.ProductId);
                if (matchingProduct == null)
                {
                    continue;
                }
                if (matchingProduct.Quantity != productToUpdate.Quantity)
                {
                    if (matchingProduct.Quantity > productToUpdate.Quantity)
                    {
                        var difference = productToUpdate.Quantity - matchingProduct.Quantity;
                        var currentStock = products.FirstOrDefault(p => p.Id == productToUpdate.ProductId);
                        if (currentStock == null)
                        {
                            continue;
                        }
                        else
                        {
                            currentStock.CurrentStock += (int)difference;
                            await _productRepositoryAsync.UpdateAsync(currentStock);
                        }
                        await _productRepositoryAsync.SaveChangesAsync();

                    }
                    else
                    {
                        var difference = productToUpdate.Quantity - matchingProduct.Quantity;
                        var currentStock = products.FirstOrDefault(p => p.Id == productToUpdate.ProductId);
                        if (currentStock == null)
                        {
                            continue;
                        }
                        else
                        {
                            currentStock.CurrentStock += (int)difference;
                            await _productRepositoryAsync.UpdateAsync(currentStock);
                        }
                        await _productRepositoryAsync.SaveChangesAsync();
                    }
                }
                productsPreExistence.Remove(productExisting);
                productsToProcess.Remove(matchingProduct);
            }
            //Remover stock de productos nuevos en la factura
            foreach (var newProductToSell in productsToProcess)
            {
                var currentStock = products.FirstOrDefault(p => p.Id == newProductToSell.ProductId);
                if (currentStock == null)
                {
                    continue;
                }
                else
                {
                    currentStock.CurrentStock -= (int)newProductToSell.Quantity;
                    await _productRepositoryAsync.UpdateAsync(currentStock);
                }
                await _productRepositoryAsync.SaveChangesAsync();

            }
            //Devolver Stock de productos eliminados de la factura
            foreach (var productPreExistence in productsPreExistence)
            {
                var currentStock = products.FirstOrDefault(p => p.Id == productPreExistence.ProductId);
                if (currentStock == null)
                {
                    continue;
                }
                else
                {
                    currentStock.CurrentStock += (int)productPreExistence.Quantity;
                    await _productRepositoryAsync.UpdateAsync(currentStock);
                }
                await _productRepositoryAsync.SaveChangesAsync();
            }
        }
    }

}
