using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.SubcategorySpecification
{
    /// <summary>
    /// Carga las subcategorias cuyo Name esta en la lista dada (resolucion por nombre en lote para
    /// la importacion masiva). El cotejo es case-insensitive por la collation de SQL Server.
    /// </summary>
    public class SubcategoriesByNamesSpecification : Specification<Subcategory>
    {
        public SubcategoriesByNamesSpecification(IEnumerable<string> names)
        {
            Query.Where(x => names.Contains(x.Name)).AsNoTracking();
        }
    }
}
