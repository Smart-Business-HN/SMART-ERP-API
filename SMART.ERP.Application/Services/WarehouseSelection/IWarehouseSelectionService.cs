using SMART.ERP.Application.DTOs.Warehouse;

namespace SMART.ERP.Application.Services.WarehouseSelection
{
    public interface IWarehouseSelectionService
    {
        Task<WarehouseSelectionResult> SelectOptimalWarehouseAsync(
            int productId,
            decimal quantity,
            int? destinationCityId = null,
            bool preferPhysical = true);

        Task<WarehouseSelectionResult?> TrySelectOptimalWarehouseAsync(
            int productId,
            decimal quantity,
            int? destinationCityId = null,
            bool preferPhysical = true);

        Task<List<WarehouseSelectionResult>> GetAvailableWarehousesAsync(
            int productId,
            decimal quantity);
    }
}
