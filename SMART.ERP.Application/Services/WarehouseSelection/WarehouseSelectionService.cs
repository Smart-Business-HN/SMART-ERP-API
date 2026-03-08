using SMART.ERP.Application.DTOs.Warehouse;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.ShippingCostCalculator;
using SMART.ERP.Application.Specifications.InventoryDistributionSpecification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Services.WarehouseSelection
{
    public class WarehouseSelectionService : IWarehouseSelectionService
    {
        private readonly IRepositoryAsync<InventoryDistribution> _inventoryRepository;
        private readonly IShippingCostCalculatorService _shippingCalculator;

        public WarehouseSelectionService(
            IRepositoryAsync<InventoryDistribution> inventoryRepository,
            IShippingCostCalculatorService shippingCalculator)
        {
            _inventoryRepository = inventoryRepository;
            _shippingCalculator = shippingCalculator;
        }

        public async Task<WarehouseSelectionResult> SelectOptimalWarehouseAsync(
            int productId,
            decimal quantity,
            int? destinationCityId = null,
            bool preferPhysical = true)
        {
            var availableWarehouses = await GetAvailableWarehousesAsync(productId, quantity);

            if (!availableWarehouses.Any())
            {
                throw new ApiException($"No hay stock disponible para el producto {productId}. Cantidad solicitada: {quantity}");
            }

            if (preferPhysical)
            {
                var physicalWarehouse = availableWarehouses
                    .Where(w => !w.IsVirtual && w.AvailableQuantity >= quantity)
                    .OrderByDescending(w => w.AvailableQuantity)
                    .FirstOrDefault();

                if (physicalWarehouse != null)
                {
                    return physicalWarehouse;
                }
            }

            var virtualWarehouses = availableWarehouses
                .Where(w => w.IsVirtual && w.AvailableQuantity >= quantity)
                .ToList();

            if (!virtualWarehouses.Any())
            {
                throw new ApiException($"Stock insuficiente para el producto {productId}. Disponible: {availableWarehouses.Sum(w => w.AvailableQuantity)}, Solicitado: {quantity}");
            }

            foreach (var warehouse in virtualWarehouses)
            {
                var shippingCost = await _shippingCalculator.CalculateShippingCostAsync(
                    productId,
                    warehouse.WarehouseId,
                    destinationCityId,
                    quantity);

                warehouse.EstimatedShippingCost = shippingCost.DefaultCost;
                warehouse.CostType = shippingCost.CostType;
            }

            var bestVirtualWarehouse = virtualWarehouses
                .OrderBy(w => w.EstimatedShippingCost ?? decimal.MaxValue)
                .ThenByDescending(w => w.AvailableQuantity)
                .FirstOrDefault();

            if (bestVirtualWarehouse != null)
            {
                return bestVirtualWarehouse;
            }

            return availableWarehouses
                .OrderByDescending(w => w.AvailableQuantity)
                .First();
        }

        public async Task<WarehouseSelectionResult?> TrySelectOptimalWarehouseAsync(
            int productId,
            decimal quantity,
            int? destinationCityId = null,
            bool preferPhysical = true)
        {
            var availableWarehouses = await GetAvailableWarehousesAsync(productId, quantity);
            if (!availableWarehouses.Any())
                return null;

            return await SelectOptimalWarehouseAsync(productId, quantity, destinationCityId, preferPhysical);
        }

        public async Task<List<WarehouseSelectionResult>> GetAvailableWarehousesAsync(
            int productId,
            decimal quantity)
        {
            var spec = new FilterInventoryByProductWithDetailsSpec(productId);
            var inventoryDistributions = await _inventoryRepository.ListAsync(spec);

            var results = new List<WarehouseSelectionResult>();

            foreach (var distribution in inventoryDistributions)
            {
                if (distribution.Warehouse == null) continue;

                var providerWarehouse = distribution.Warehouse.ProviderWarehouses?.FirstOrDefault();

                results.Add(new WarehouseSelectionResult
                {
                    WarehouseId = distribution.WarehouseId,
                    WarehouseName = distribution.Warehouse.Name,
                    IsVirtual = distribution.Warehouse.IsVirtual,
                    AvailableQuantity = distribution.Quantity,
                    ProviderId = providerWarehouse?.ProviderId,
                    ProviderName = providerWarehouse?.Provider?.Name
                });
            }

            return results;
        }
    }
}
