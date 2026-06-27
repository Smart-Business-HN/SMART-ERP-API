using Ardalis.Specification;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Domain.Enums;

namespace SMART.ERP.Application.Specifications.ReportSpecification
{
    /// <summary>
    /// Productos que "tocan inventario": tangibles, activos y no eliminados.
    /// Incluye subcategoria y unidad de medida para armar la sugerencia de compra.
    /// </summary>
    public class PurchaseSuggestionProductsSpecification : Specification<Product>
    {
        public PurchaseSuggestionProductsSpecification()
        {
            Query
                .Where(x => x.ProductType == ProductType.Tangible && x.IsActive && !x.IsDeleted)
                .Include(x => x.SubCategory)
                .Include(x => x.UnitOfMeasurement)
                .AsNoTracking();
        }
    }
}
