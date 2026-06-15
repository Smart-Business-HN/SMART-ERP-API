using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.SubcategorySpecification
{
    /// <summary>
    /// Carga las subcategorías cuyos ids están en la lista dada (para validar existencia en lote).
    /// </summary>
    public class SubcategoriesByIdsSpecification : Specification<Subcategory>
    {
        public SubcategoriesByIdsSpecification(IEnumerable<int> ids)
        {
            Query.Where(x => ids.Contains(x.Id));
        }
    }
}
