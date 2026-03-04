using SMART.ERP.Application.DTOs.Product;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Services
{
    public interface IProductPricingService
    {
        /// <summary>
        /// Calcula el precio recomendado de venta para un producto basado en el tipo de cliente
        /// </summary>
        /// <param name="product">El producto para calcular el precio</param>
        /// <param name="isUserSignedIn">Indica si el usuario está autenticado</param>
        /// <param name="customerTypeId">ID del tipo de cliente (opcional)</param>
        /// <returns>El precio recomendado de venta calculado</returns>
        decimal CalculateRecommendedSalePrice(Product product, bool isUserSignedIn, int? customerTypeId = null);

        /// <summary>
        /// Calcula el precio recomendado de venta incluyendo costos de envío para dropshipping
        /// </summary>
        /// <param name="product">El producto para calcular el precio</param>
        /// <param name="isUserSignedIn">Indica si el usuario está autenticado</param>
        /// <param name="customerTypeId">ID del tipo de cliente (opcional)</param>
        /// <param name="sourceWarehouseId">ID del almacén de origen (opcional, se selecciona automáticamente si no se especifica)</param>
        /// <param name="destinationCityId">ID de la ciudad de destino (opcional)</param>
        /// <returns>Precio con desglose de shipping y total</returns>
        Task<ProductPriceWithShippingDto> CalculateRecommendedSalePriceWithShippingAsync(
            Product product,
            bool isUserSignedIn,
            int? customerTypeId = null,
            int? sourceWarehouseId = null,
            int? destinationCityId = null);
    }
}
