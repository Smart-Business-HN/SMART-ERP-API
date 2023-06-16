using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.MachinerySpecification
{
    public class FilterRootcloudHistoricalByIdSpecification : Specification<Machinery>
    {
        public FilterRootcloudHistoricalByIdSpecification(int id)
        {
            Query.Include(i => i.MachineyRootcloudHistoricals!.OrderByDescending(o => o.Id).Take(1))
                .Where(x => x.Id == id).AsNoTracking();
        }
    }
}
