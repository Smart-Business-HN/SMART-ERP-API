using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.FinancingPlanSpecification
{
    public class FilterFinancingPlanSpecification : Specification<FinancingPlan>
    {
        public FilterFinancingPlanSpecification(string filter, int? id)
        {
            if (id != null) Query.Where(x => x.Name == filter && x.Id != id).AsNoTracking();
            else Query.Where(x => x.Name == filter).AsNoTracking();
        }
    }
}
