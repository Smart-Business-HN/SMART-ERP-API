using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.TypeActivitySpecification
{
    public class FilterTypeActivitySpecification : Specification<TypeActivity>
    {
        public FilterTypeActivitySpecification(string filter, int? id)
        {
            if (id != null) Query.Where(x => x.Name == filter && x.Id != id).AsNoTracking();
            else Query.Where(x => x.Name == filter).AsNoTracking();
        }
    }
}
