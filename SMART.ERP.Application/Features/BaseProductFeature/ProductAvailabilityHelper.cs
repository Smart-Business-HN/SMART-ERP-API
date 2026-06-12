using SMART.ERP.Application.DTOs.Product;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.BaseProductFeature
{
    /// <summary>
    /// Calcula la disponibilidad de ecommerce (EcommerceStock) de cada producto como la suma de TODAS
    /// sus distribuciones de inventario, incluyendo almacenes virtuales (consignados/dropshipping).
    /// Esto es independiente de Product.CurrentStock (solo-físico, para reportes/contabilidad): el stock
    /// virtual hace aparecer el producto como disponible en la tienda SIN tocar el inventario propio.
    /// </summary>
    public static class ProductAvailabilityHelper
    {
        public static void ApplyEcommerceStock(IEnumerable<ProductDto> products, IEnumerable<InventoryDistribution> distributions)
        {
            var stockByProduct = distributions
                .GroupBy(d => d.ProductId)
                .ToDictionary(g => g.Key, g => (int)g.Sum(d => d.Quantity));

            foreach (var product in products)
            {
                product.EcommerceStock = stockByProduct.TryGetValue(product.Id, out var quantity) ? quantity : 0;
            }
        }
    }
}
