using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.ProductSubcategorySpecification
{
    /// <summary>
    /// Carga las filas de la tabla puente de un producto para reconciliar sus subcategorías.
    /// </summary>
    public class ProductSubcategoriesByProductSpecification : Specification<ProductSubcategory>
    {
        public ProductSubcategoriesByProductSpecification(int productId)
        {
            Query.Where(x => x.ProductId == productId);
        }
    }
}
