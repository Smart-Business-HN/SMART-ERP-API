using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.StatusSpecification
{
    public class FilterStatusFromNameExceptIdSpecification : Specification<Status>
    {
        public FilterStatusFromNameExceptIdSpecification(string name, int id)
        {
            Query.Where(x => x.Name == name && x.Id != id).AsNoTracking();
        }
    }
}
