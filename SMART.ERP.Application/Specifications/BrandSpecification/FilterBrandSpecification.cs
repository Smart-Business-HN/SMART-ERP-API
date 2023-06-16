using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.BrandSpecification
{
    public class FilterBrandSpecification : Specification<Brand>
    {
        public FilterBrandSpecification(string filter, int? id)
        {
            if (id != null) Query.Where(x => x.Name == filter && x.Id != id).AsNoTracking();
            else Query.Where(x => x.Name == filter).AsNoTracking();
        }
    }
}
