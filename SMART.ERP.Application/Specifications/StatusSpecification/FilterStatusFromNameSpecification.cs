using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.StatusSpecification
{
    public class FilterStatusFromNameSpecification : Specification<Status>
    {
        public FilterStatusFromNameSpecification(string name)
        {
            Query.Where(x => x.Name == name).AsNoTracking();
        }
    }
}
