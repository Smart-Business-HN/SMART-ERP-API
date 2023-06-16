using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.CategorySpecification
{
    public class GetCategoriesNotPartsSpecification : Specification<Category>
    {
        public GetCategoriesNotPartsSpecification()
        {
            Query.Include(a => a.Subcategories).Where(x => x.Name != "Maquinaria Liviana" && !x.IsPartCategory).AsNoTracking();
        }
    }
}
