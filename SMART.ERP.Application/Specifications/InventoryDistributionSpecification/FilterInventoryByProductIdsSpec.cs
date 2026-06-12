using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.InventoryDistributionSpecification
{
    /// <summary>
    /// Trae las distribuciones de inventario (de TODOS los almacenes, físicos y virtuales)
    /// para un conjunto de productos. Se usa para calcular la disponibilidad de ecommerce
    /// (físico + virtual) en batch, sin tocar Product.CurrentStock (que es solo-físico).
    /// </summary>
    public class FilterInventoryByProductIdsSpec : Specification<InventoryDistribution>
    {
        public FilterInventoryByProductIdsSpec(IEnumerable<int> productIds)
        {
            Query.Where(d => productIds.Contains(d.ProductId)).AsNoTracking();
        }
    }
}
