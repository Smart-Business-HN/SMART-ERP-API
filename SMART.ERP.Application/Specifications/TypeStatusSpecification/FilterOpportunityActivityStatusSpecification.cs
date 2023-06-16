using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.TypeStatusSpecification
{
    public class FilterOpportunityActivityStatusSpecification : Specification<TypeStatus>
    {
        public FilterOpportunityActivityStatusSpecification()
        {
            Query.Where(x => x.Name == "Actividad").Include(x => x.Status).AsNoTracking();
        }
    }
}
