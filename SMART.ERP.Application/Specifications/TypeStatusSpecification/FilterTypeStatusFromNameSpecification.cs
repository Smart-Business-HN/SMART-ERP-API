using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.TypeStatusSpecification
{
    public class FilterTypeStatusFromNameSpecification : Specification<TypeStatus>
    {
        public FilterTypeStatusFromNameSpecification(string name, int? id)
        {
            Query.Where(x => x.Name == name).AsNoTracking();
            if (id != null)
            {
                Query.Where(x => x.Id != id);
            }
        }
    }
}
