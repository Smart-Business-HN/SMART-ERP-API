using SMART.ERP.Application.DTOs.Product;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Services
{
    public interface IProductPricingService
    {
        /// <summary>
        /// Resuelve el precio de venta para un producto usando la lista de precios aplicable al cliente.
        /// </summary>
        Task<decimal> CalculateRecommendedSalePriceAsync(
            Product product,
            bool isUserSignedIn,
            int? customerTypeId = null,
            Guid? customerId = null,
            CancellationToken ct = default);

        /// <summary>
        /// Resuelve precios para un conjunto de productos en batch (evita N+1).
        /// </summary>
        Task<IReadOnlyDictionary<int, decimal>> CalculateRecommendedSalePricesAsync(
            IEnumerable<int> productIds,
            bool isUserSignedIn,
            int? customerTypeId = null,
            Guid? customerId = null,
            CancellationToken ct = default);

        /// <summary>
        /// Resuelve el precio de venta incluyendo costos de envío para dropshipping.
        /// </summary>
        Task<ProductPriceWithShippingDto> CalculateRecommendedSalePriceWithShippingAsync(
            Product product,
            bool isUserSignedIn,
            int? customerTypeId = null,
            int? sourceWarehouseId = null,
            int? destinationCityId = null,
            Guid? customerId = null,
            CancellationToken ct = default);
    }
}
