using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.MachinerySpecification
{
    public class GetEndRootcloudHistoricalSpecification : Specification<MachineryRootcloudHistorical>
    {
        public GetEndRootcloudHistoricalSpecification(int machineryId)
        {
            Query.Where(x => x.MachineryId == machineryId).OrderByDescending(o => o.Id);
        }
    }
}
