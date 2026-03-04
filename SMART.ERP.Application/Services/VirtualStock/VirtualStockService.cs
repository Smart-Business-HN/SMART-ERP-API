using SMART.ERP.Application.DTOs.Product;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.InventoryDistributionSpecification;
using SMART.ERP.Application.Specifications.ProductSpecification;
using SMART.ERP.Application.Specifications.ProviderWarehouseSpecification;
using SMART.ERP.Domain.Entities;
using System.Globalization;
using System.Text;

namespace SMART.ERP.Application.Services.VirtualStock
{
    public class VirtualStockService : IVirtualStockService
    {
        private readonly IRepositoryAsync<Provider> _providerRepository;
        private readonly IRepositoryAsync<Warehouse> _warehouseRepository;
        private readonly IRepositoryAsync<Product> _productRepository;
        private readonly IRepositoryAsync<InventoryDistribution> _inventoryRepository;
        private readonly IRepositoryAsync<VirtualStockImport> _importRepository;
        private readonly IRepositoryAsync<ProviderWarehouse> _providerWarehouseRepository;

        public VirtualStockService(
            IRepositoryAsync<Provider> providerRepository,
            IRepositoryAsync<Warehouse> warehouseRepository,
            IRepositoryAsync<Product> productRepository,
            IRepositoryAsync<InventoryDistribution> inventoryRepository,
            IRepositoryAsync<VirtualStockImport> importRepository,
            IRepositoryAsync<ProviderWarehouse> providerWarehouseRepository)
        {
            _providerRepository = providerRepository;
            _warehouseRepository = warehouseRepository;
            _productRepository = productRepository;
            _inventoryRepository = inventoryRepository;
            _importRepository = importRepository;
            _providerWarehouseRepository = providerWarehouseRepository;
        }

        public async Task<VirtualStockImport> ImportStockFromCsvAsync(
            int providerId,
            int warehouseId,
            Stream fileStream,
            string fileName,
            string importedBy)
        {
            var provider = await _providerRepository.GetByIdAsync(providerId);
            if (provider == null)
            {
                throw new ApiException($"Proveedor con ID {providerId} no encontrado");
            }

            var warehouse = await _warehouseRepository.GetByIdAsync(warehouseId);
            if (warehouse == null)
            {
                throw new ApiException($"Almacén con ID {warehouseId} no encontrado");
            }

            if (!warehouse.IsVirtual)
            {
                throw new ApiException($"El almacén {warehouse.Name} no es un almacén virtual");
            }

            var import = new VirtualStockImport
            {
                ProviderId = providerId,
                WarehouseId = warehouseId,
                FileName = fileName,
                ImportDate = DateTime.UtcNow,
                ImportedBy = importedBy,
                ImportDetails = new List<VirtualStockImportDetail>()
            };

            var importDetails = new List<VirtualStockImportDetail>();
            int successCount = 0;
            int failCount = 0;
            var errorLog = new StringBuilder();

            using (var reader = new StreamReader(fileStream))
            {
                string? headerLine = await reader.ReadLineAsync();
                if (headerLine == null)
                {
                    throw new ApiException("El archivo CSV está vacío");
                }

                int lineNumber = 1;
                while (!reader.EndOfStream)
                {
                    lineNumber++;
                    var line = await reader.ReadLineAsync();
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    var detail = await ProcessCsvLineAsync(line, lineNumber, warehouseId);
                    importDetails.Add(detail);

                    if (detail.WasSuccessful)
                    {
                        successCount++;
                    }
                    else
                    {
                        failCount++;
                        errorLog.AppendLine($"Línea {lineNumber}: {detail.ErrorMessage}");
                    }
                }
            }

            import.TotalProducts = importDetails.Count;
            import.SuccessfulImports = successCount;
            import.FailedImports = failCount;
            import.ErrorLog = errorLog.ToString();
            import.ImportDetails = importDetails;

            await _importRepository.AddAsync(import);

            return import;
        }

        public async Task<VirtualStockImport> ImportStockFromExcelAsync(
            int providerId,
            int warehouseId,
            Stream fileStream,
            string fileName,
            string importedBy)
        {
            throw new NotImplementedException("Importación de Excel no implementada aún. Use CSV.");
        }

        public async Task<bool> SyncProductWithProviderStockAsync(
            int productId,
            int providerId,
            decimal quantity)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null)
            {
                throw new ApiException($"Producto con ID {productId} no encontrado");
            }

            var spec = new FilterProviderWarehouseByProviderSpec(providerId);
            var providerWarehouse = await _providerWarehouseRepository.FirstOrDefaultAsync(spec);

            if (providerWarehouse == null)
            {
                throw new ApiException($"No existe almacén virtual asociado al proveedor {providerId}");
            }

            var distributionSpec = new FilterInventoryByProductAndWarehouseSpec(productId, providerWarehouse.WarehouseId);
            var distribution = await _inventoryRepository.FirstOrDefaultAsync(distributionSpec);

            if (distribution == null)
            {
                distribution = new InventoryDistribution
                {
                    ProductId = productId,
                    WarehouseId = providerWarehouse.WarehouseId,
                    Quantity = quantity
                };
                await _inventoryRepository.AddAsync(distribution);
            }
            else
            {
                distribution.Quantity = quantity;
                await _inventoryRepository.UpdateAsync(distribution);
            }

            await RecalculateProductCurrentStockAsync(productId);

            return true;
        }

        public async Task<ProductAvailabilityDto> GetProductAvailabilityAsync(int productId)
        {
            var productSpec = new FilterProductByIdWithInventorySpec(productId);
            var product = await _productRepository.FirstOrDefaultAsync(productSpec);

            if (product == null)
            {
                throw new ApiException($"Producto con ID {productId} no encontrado");
            }

            var result = new ProductAvailabilityDto
            {
                ProductId = product.Id,
                ProductCode = product.Code,
                ProductName = product.Name,
                TotalAvailableStock = product.CurrentStock,
                StockByWarehouse = new List<WarehouseStockDto>()
            };

            if (product.InventoryDistributions != null)
            {
                foreach (var distribution in product.InventoryDistributions.Where(d => d.Quantity > 0))
                {
                    if (distribution.Warehouse == null) continue;

                    var providerWarehouse = distribution.Warehouse.ProviderWarehouses?.FirstOrDefault();

                    var stockDto = new WarehouseStockDto
                    {
                        WarehouseId = distribution.WarehouseId,
                        WarehouseName = distribution.Warehouse.Name,
                        WarehouseTypeName = distribution.Warehouse.IsVirtual ? "Virtual" : "Físico",
                        IsVirtual = distribution.Warehouse.IsVirtual,
                        Quantity = distribution.Quantity,
                        ProviderName = providerWarehouse?.Provider?.Name,
                        CityName = distribution.Warehouse.City?.Name
                    };

                    result.StockByWarehouse.Add(stockDto);

                    if (distribution.Warehouse.IsVirtual)
                    {
                        result.TotalVirtualStock += distribution.Quantity;
                    }
                    else
                    {
                        result.TotalPhysicalStock += distribution.Quantity;
                    }
                }
            }

            return result;
        }

        private async Task<VirtualStockImportDetail> ProcessCsvLineAsync(
            string line,
            int lineNumber,
            int warehouseId)
        {
            var detail = new VirtualStockImportDetail
            {
                WasSuccessful = false
            };

            try
            {
                var values = line.Split(',');
                if (values.Length < 2)
                {
                    detail.ErrorMessage = "Formato inválido. Se esperan al menos 2 columnas: ProductCode,Quantity";
                    detail.ProductCode = line.Substring(0, Math.Min(50, line.Length));
                    return detail;
                }

                string productCode = values[0].Trim();
                string quantityStr = values[1].Trim();
                decimal? costPrice = null;

                if (values.Length >= 3 && !string.IsNullOrWhiteSpace(values[2]))
                {
                    if (decimal.TryParse(values[2].Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal cp))
                    {
                        costPrice = cp;
                    }
                }

                detail.ProductCode = productCode;

                if (!decimal.TryParse(quantityStr, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal quantity))
                {
                    detail.ErrorMessage = $"Cantidad inválida: {quantityStr}";
                    return detail;
                }

                detail.Quantity = quantity;
                detail.CostPrice = costPrice;

                var productSpec = new FilterProductByCodeSpec(productCode);
                var product = await _productRepository.FirstOrDefaultAsync(productSpec);

                if (product == null)
                {
                    detail.ErrorMessage = $"Producto con código {productCode} no encontrado";
                    return detail;
                }

                detail.ProductId = product.Id;

                var distributionSpec = new FilterInventoryByProductAndWarehouseSpec(product.Id, warehouseId);
                var distribution = await _inventoryRepository.FirstOrDefaultAsync(distributionSpec);

                if (distribution == null)
                {
                    distribution = new InventoryDistribution
                    {
                        ProductId = product.Id,
                        WarehouseId = warehouseId,
                        Quantity = quantity
                    };
                    await _inventoryRepository.AddAsync(distribution);
                }
                else
                {
                    distribution.Quantity = quantity;
                    await _inventoryRepository.UpdateAsync(distribution);
                }

                await RecalculateProductCurrentStockAsync(product.Id);

                detail.WasSuccessful = true;
            }
            catch (Exception ex)
            {
                detail.ErrorMessage = $"Error procesando línea: {ex.Message}";
                detail.WasSuccessful = false;
            }

            return detail;
        }

        private async Task RecalculateProductCurrentStockAsync(int productId)
        {
            var inventorySpec = new FilterInventoryByProductSpec(productId);
            var inventoryDistributions = await _inventoryRepository.ListAsync(inventorySpec);
            var totalStock = inventoryDistributions.Sum(d => d.Quantity);

            var product = await _productRepository.GetByIdAsync(productId);
            if (product != null)
            {
                product.CurrentStock = (int)totalStock;
                await _productRepository.UpdateAsync(product);
            }
        }
    }
}
