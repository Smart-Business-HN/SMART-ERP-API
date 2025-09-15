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
    }
}
