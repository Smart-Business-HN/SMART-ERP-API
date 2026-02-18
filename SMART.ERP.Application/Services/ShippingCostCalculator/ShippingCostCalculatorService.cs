using SMART.ERP.Application.DTOs.ShippingCost;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.ShippingCostConfigurationSpecification;
using SMART.ERP.Application.Specifications.WarehouseSpecification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Services.ShippingCostCalculator
{
    public class ShippingCostCalculatorService : IShippingCostCalculatorService
    {
        private readonly IRepositoryAsync<Warehouse> _warehouseRepository;
        private readonly IRepositoryAsync<Provider> _providerRepository;
        private readonly IRepositoryAsync<ShippingCostConfiguration> _shippingCostRepository;

        public ShippingCostCalculatorService(
            IRepositoryAsync<Warehouse> warehouseRepository,
            IRepositoryAsync<Provider> providerRepository,
            IRepositoryAsync<ShippingCostConfiguration> shippingCostRepository)
        {
            _warehouseRepository = warehouseRepository;
            _providerRepository = providerRepository;
            _shippingCostRepository = shippingCostRepository;
        }

        public async Task<ShippingCostResult> CalculateShippingCostAsync(
            int productId,
            int? sourceWarehouseId,
            int? destinationCityId,
            decimal quantity = 1)
        {
            if (!sourceWarehouseId.HasValue)
            {
                return new ShippingCostResult
                {
                    MinCost = 0,
                    MaxCost = 0,
                    DefaultCost = 0,
                    CostType = "None",
                    IsFromVirtualStock = false
                };
            }

            var warehouse = await _warehouseRepository
                .FirstOrDefaultAsync(new FilterWarehouseByIdWithProviderSpec(sourceWarehouseId.Value));

            if (warehouse == null || !warehouse.IsVirtual)
            {
                return new ShippingCostResult
                {
                    MinCost = 0,
                    MaxCost = 0,
                    DefaultCost = 0,
                    CostType = "None",
                    SourceWarehouseId = sourceWarehouseId,
                    SourceWarehouseName = warehouse?.Name,
                    IsFromVirtualStock = false
                };
            }

            var providerWarehouse = warehouse.ProviderWarehouses?.FirstOrDefault();
            if (providerWarehouse == null)
            {
                return new ShippingCostResult
                {
                    MinCost = 0,
                    MaxCost = 0,
                    DefaultCost = 0,
                    CostType = "Unknown",
                    SourceWarehouseId = sourceWarehouseId,
                    SourceWarehouseName = warehouse.Name,
                    IsFromVirtualStock = true
                };
            }

            var configurations = await GetApplicableShippingRatesAsync(
                sourceWarehouseId,
                providerWarehouse.ProviderId,
                destinationCityId);

            var bestConfig = configurations
                .Where(c => c.IsActive && (!c.ProductId.HasValue || c.ProductId == productId))
                .OrderByDescending(c => c.Priority)
                .ThenByDescending(c => c.ProductId.HasValue ? 1 : 0)
                .FirstOrDefault();

            if (bestConfig != null)
            {
                return new ShippingCostResult
                {
                    MinCost = bestConfig.MinCost,
                    MaxCost = bestConfig.MaxCost,
                    DefaultCost = bestConfig.DefaultCost,
                    CostType = bestConfig.CostType,
                    SourceWarehouseId = sourceWarehouseId,
                    SourceWarehouseName = warehouse.Name,
                    SourceProviderId = providerWarehouse.ProviderId,
                    SourceProviderName = providerWarehouse.Provider?.Name,
                    IsFromVirtualStock = true
                };
            }

            var provider = providerWarehouse.Provider;
            if (provider?.DefaultShippingCost.HasValue == true)
            {
                return new ShippingCostResult
                {
                    MinCost = provider.DefaultShippingCost.Value,
                    MaxCost = provider.DefaultShippingCost.Value,
                    DefaultCost = provider.DefaultShippingCost.Value,
                    CostType = provider.DefaultShippingType ?? "Delivery",
                    SourceWarehouseId = sourceWarehouseId,
                    SourceWarehouseName = warehouse.Name,
                    SourceProviderId = providerWarehouse.ProviderId,
                    SourceProviderName = provider.Name,
                    IsFromVirtualStock = true
                };
            }

            return new ShippingCostResult
            {
                MinCost = 0,
                MaxCost = 0,
                DefaultCost = 0,
                CostType = "Unknown",
                SourceWarehouseId = sourceWarehouseId,
                SourceWarehouseName = warehouse.Name,
                SourceProviderId = providerWarehouse.ProviderId,
                SourceProviderName = provider?.Name,
                IsFromVirtualStock = true
            };
        }

        public async Task<ShippingCostResult> CalculateShippingCostByProviderAsync(
            int providerId,
            int? destinationCityId,
            decimal quantity = 1)
        {
            var configurations = await GetApplicableShippingRatesAsync(
                null,
                providerId,
                destinationCityId);

            var bestConfig = configurations
                .Where(c => c.IsActive)
                .OrderByDescending(c => c.Priority)
                .FirstOrDefault();

            if (bestConfig != null)
            {
                return new ShippingCostResult
                {
                    MinCost = bestConfig.MinCost,
                    MaxCost = bestConfig.MaxCost,
                    DefaultCost = bestConfig.DefaultCost,
                    CostType = bestConfig.CostType,
                    SourceProviderId = providerId,
                    IsFromVirtualStock = true
                };
            }

            var provider = await _providerRepository.GetByIdAsync(providerId);
            if (provider?.DefaultShippingCost.HasValue == true)
            {
                return new ShippingCostResult
                {
                    MinCost = provider.DefaultShippingCost.Value,
                    MaxCost = provider.DefaultShippingCost.Value,
                    DefaultCost = provider.DefaultShippingCost.Value,
                    CostType = provider.DefaultShippingType ?? "Delivery",
                    SourceProviderId = providerId,
                    SourceProviderName = provider.Name,
                    IsFromVirtualStock = true
                };
            }

            return new ShippingCostResult
            {
                MinCost = 0,
                MaxCost = 0,
                DefaultCost = 0,
                CostType = "Unknown",
                SourceProviderId = providerId,
                IsFromVirtualStock = true
            };
        }

        public async Task<Dictionary<int, decimal>> CalculateConsolidatedShippingByProviderAsync(
            Dictionary<int, List<int>> productsByProvider,
            int? destinationCityId = null)
        {
            var result = new Dictionary<int, decimal>();

            foreach (var providerGroup in productsByProvider)
            {
                int providerId = providerGroup.Key;
                var shippingCost = await CalculateShippingCostByProviderAsync(
                    providerId,
                    destinationCityId);

                result[providerId] = shippingCost.DefaultCost;
            }

            return result;
        }

        public async Task<List<ShippingCostConfiguration>> GetApplicableShippingRatesAsync(
            int? sourceWarehouseId,
            int? sourceProviderId,
            int? destinationCityId)
        {
            var spec = new FilterActiveShippingCostsSpec(sourceWarehouseId, sourceProviderId, destinationCityId);
            return await _shippingCostRepository.ListAsync(spec);
        }
    }
}
