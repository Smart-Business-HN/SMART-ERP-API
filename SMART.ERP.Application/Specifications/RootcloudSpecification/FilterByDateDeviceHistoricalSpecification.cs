using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.RootcloudSpecification
{
    public class FilterByDateDeviceHistoricalSpecification : Specification<MachineryRootcloudHistorical>
    {
        public FilterByDateDeviceHistoricalSpecification(int machineId, DateTime date)
        {
            Query.Where(a => a.MachineryId == machineId && a.CreationDate.Date == date.Date);
        }
    }
}
