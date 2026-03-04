using SMART.ERP.Application.DTOs.Product;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Services.VirtualStock
{
    public interface IVirtualStockService
    {
        Task<VirtualStockImport> ImportStockFromCsvAsync(
            int providerId,
            int warehouseId,
            Stream fileStream,
            string fileName,
            string importedBy);

        Task<VirtualStockImport> ImportStockFromExcelAsync(
            int providerId,
            int warehouseId,
            Stream fileStream,
            string fileName,
            string importedBy);

        Task<bool> SyncProductWithProviderStockAsync(
            int productId,
            int providerId,
            decimal quantity);

        Task<ProductAvailabilityDto> GetProductAvailabilityAsync(int productId);
    }
}
