using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.MachinerySpecification
{
    public class FilterMachineryByHistoricalSpecification : Specification<Machinery>
    {
        public FilterMachineryByHistoricalSpecification()
        {
            Query.Include(i => i.MachineyRootcloudHistoricals!.OrderByDescending(o => o.Id).Take(1));
        }
    }
}
