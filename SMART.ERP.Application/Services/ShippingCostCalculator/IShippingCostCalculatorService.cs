using SMART.ERP.Application.DTOs.ShippingCost;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Services.ShippingCostCalculator
{
    public interface IShippingCostCalculatorService
    {
        Task<ShippingCostResult> CalculateShippingCostAsync(
            int productId,
            int? sourceWarehouseId,
            int? destinationCityId,
            decimal quantity = 1);

        Task<ShippingCostResult> CalculateShippingCostByProviderAsync(
            int providerId,
            int? destinationCityId,
            decimal quantity = 1);

        Task<Dictionary<int, decimal>> CalculateConsolidatedShippingByProviderAsync(
            Dictionary<int, List<int>> productsByProvider,
            int? destinationCityId = null);

        Task<List<ShippingCostConfiguration>> GetApplicableShippingRatesAsync(
            int? sourceWarehouseId,
            int? sourceProviderId,
            int? destinationCityId);
    }
}
