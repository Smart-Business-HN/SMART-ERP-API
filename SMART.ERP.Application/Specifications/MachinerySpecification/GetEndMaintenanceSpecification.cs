using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.MachinerySpecification
{
    public class GetEndMaintenanceSpecification : Specification<MachineryRootcloudHistorical>
    {
        public GetEndMaintenanceSpecification(int machineId, DateTime creationDate)
        {
            Query.Where(x => x.MachineryId == machineId && x.CreationDate.Date == creationDate.Date);
        }
    }
}
