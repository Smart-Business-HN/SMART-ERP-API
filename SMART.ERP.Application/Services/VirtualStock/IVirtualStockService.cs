using SMART.ERP.Application.DTOs.Product;
using SMART.ERP.Application.DTOs.VirtualStock;
using SMART.ERP.Application.Wrappers;
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

        // Historial paginado de importaciones. providerId null o <= 0 = todos los proveedores.
        Task<PagedResponse<List<VirtualStockImportHistoryItemDto>>> GetImportHistoryAsync(
            int? providerId,
            int pageNumber,
            int pageSize);
    }
}
