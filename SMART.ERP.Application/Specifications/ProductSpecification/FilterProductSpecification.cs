using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.ProductSpecification
{
    public class FilterProductSpecification : Specification<Product>
    {
        public FilterProductSpecification(string? filter, int? id, string? slug)
        {
            if (filter == null && id != null)
                Query.Where(x => x.Id == id).AsNoTracking();
            else if (id != null && filter != null)
                Query.Where(x => x.Code == filter && x.Id != id || x.Name == filter && x.Id != id).AsNoTracking();
            else if (id == null && filter != null && slug!=null)
                Query.Where(x=>x.Slug == slug || x.Name == filter).AsNoTracking();  
            else
                Query.Where(x => x.Code == filter || x.Name == filter).AsNoTracking();
        }
    }
}
