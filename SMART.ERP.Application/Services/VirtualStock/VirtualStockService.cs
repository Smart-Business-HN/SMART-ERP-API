using Microsoft.AspNetCore.OutputCaching;
using SMART.ERP.Application.DTOs.Product;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Features.BaseProductFeature;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.InventoryDistributionSpecification;
using SMART.ERP.Application.Specifications.ProductSpecification;
using SMART.ERP.Application.Specifications.ProviderWarehouseSpecification;
using SMART.ERP.Application.Specifications.WarehouseSpecification;
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
        private readonly IVirtualStockExcelReader _excelReader;
        private readonly IOutputCacheStore _outputCacheStore;

        public VirtualStockService(
            IRepositoryAsync<Provider> providerRepository,
            IRepositoryAsync<Warehouse> warehouseRepository,
            IRepositoryAsync<Product> productRepository,
            IRepositoryAsync<InventoryDistribution> inventoryRepository,
            IRepositoryAsync<VirtualStockImport> importRepository,
            IRepositoryAsync<ProviderWarehouse> providerWarehouseRepository,
            IVirtualStockExcelReader excelReader,
            IOutputCacheStore outputCacheStore)
        {
            _providerRepository = providerRepository;
            _warehouseRepository = warehouseRepository;
            _productRepository = productRepository;
            _inventoryRepository = inventoryRepository;
            _importRepository = importRepository;
            _providerWarehouseRepository = providerWarehouseRepository;
            _excelReader = excelReader;
            _outputCacheStore = outputCacheStore;
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

            // Evicta el caché del catálogo para que el ecommerce refleje la nueva disponibilidad virtual.
            await ProductCacheEviction.EvictAsync(_outputCacheStore, CancellationToken.None);

            return import;
        }

        public async Task<VirtualStockImport> ImportStockFromExcelAsync(
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

            var rows = _excelReader.ReadRows(fileStream);
            if (rows.Count == 0)
            {
                throw new ApiException("El archivo Excel está vacío o no contiene filas de datos (se omite la primera fila de encabezado)");
            }

            foreach (var row in rows)
            {
                var detail = await ProcessRowAsync(row.ProductCode, row.Quantity, row.CostPrice, row.RowNumber, warehouseId);
                importDetails.Add(detail);

                if (detail.WasSuccessful)
                {
                    successCount++;
                }
                else
                {
                    failCount++;
                    errorLog.AppendLine($"Línea {row.RowNumber}: {detail.ErrorMessage}");
                }
            }

            import.TotalProducts = importDetails.Count;
            import.SuccessfulImports = successCount;
            import.FailedImports = failCount;
            import.ErrorLog = errorLog.ToString();
            import.ImportDetails = importDetails;

            await _importRepository.AddAsync(import);

            // Evicta el caché del catálogo para que el ecommerce refleje la nueva disponibilidad virtual.
            await ProductCacheEviction.EvictAsync(_outputCacheStore, CancellationToken.None);

            return import;
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
            var values = line.Split(',');
            if (values.Length < 2)
            {
                return new VirtualStockImportDetail
                {
                    WasSuccessful = false,
                    ProductCode = line.Substring(0, Math.Min(50, line.Length)),
                    ErrorMessage = "Formato inválido. Se esperan al menos 2 columnas: ProductCode,Quantity"
                };
            }

            string productCode = values[0].Trim();
            string quantityStr = values[1].Trim();
            string? costPriceStr = values.Length >= 3 ? values[2].Trim() : null;

            return await ProcessRowAsync(productCode, quantityStr, costPriceStr, lineNumber, warehouseId);
        }

        /// <summary>
        /// Procesa una fila de stock virtual (ProductCode, Quantity, CostPrice opcional). Compartido por la
        /// importación CSV y Excel. La cantidad es ABSOLUTA (sobrescribe la existencia del producto en el
        /// almacén virtual). El CostPrice es solo informativo (auditoría): NO toca el costo del producto ni la
        /// contabilidad. Solo escribe en InventoryDistribution y recalcula CurrentStock (solo-físico).
        /// </summary>
        private async Task<VirtualStockImportDetail> ProcessRowAsync(
            string productCode,
            string quantityStr,
            string? costPriceStr,
            int lineNumber,
            int warehouseId)
        {
            var detail = new VirtualStockImportDetail
            {
                WasSuccessful = false
            };

            try
            {
                productCode = (productCode ?? string.Empty).Trim();
                quantityStr = (quantityStr ?? string.Empty).Trim();
                detail.ProductCode = productCode;

                if (string.IsNullOrWhiteSpace(productCode))
                {
                    detail.ErrorMessage = "Código de producto vacío";
                    return detail;
                }

                decimal? costPrice = null;
                if (!string.IsNullOrWhiteSpace(costPriceStr) &&
                    decimal.TryParse(costPriceStr.Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal cp))
                {
                    costPrice = cp;
                }

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
                detail.ErrorMessage = $"Error procesando línea {lineNumber}: {ex.Message}";
                detail.WasSuccessful = false;
            }

            return detail;
        }

        private async Task RecalculateProductCurrentStockAsync(int productId)
        {
            var inventorySpec = new FilterInventoryByProductSpec(productId);
            var inventoryDistributions = await _inventoryRepository.ListAsync(inventorySpec);

            // CurrentStock = inventario PROPIO (solo-físico): se excluyen los almacenes virtuales (consignados)
            // para no inflar el inventario propio ni los reportes. La disponibilidad de ecommerce (físico + virtual)
            // se calcula aparte en los handlers de ecommerce. Consistente con InventoryMovementService.SyncProductStockAsync.
            var warehouseIds = inventoryDistributions.Select(d => d.WarehouseId).Distinct().ToList();
            var virtualWarehouseIds = (await _warehouseRepository.ListAsync(new FilterVirtualWarehousesByIdsSpec(warehouseIds)))
                .Select(w => w.Id)
                .ToHashSet();
            var totalStock = inventoryDistributions
                .Where(d => !virtualWarehouseIds.Contains(d.WarehouseId))
                .Sum(d => d.Quantity);

            var product = await _productRepository.GetByIdAsync(productId);
            if (product != null)
            {
                product.CurrentStock = (int)totalStock;
                await _productRepository.UpdateAsync(product);
            }
        }
    }
}
