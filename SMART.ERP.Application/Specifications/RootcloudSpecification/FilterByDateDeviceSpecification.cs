using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.RootcloudSpecification
{
    public class FilterByDateDeviceSpecification : Specification<MachineryRootcloudHistorical>
    {
        public FilterByDateDeviceSpecification(string date)
        {
            Query.Where(a => a.CreationDate.Date == Convert.ToDateTime(date).Date);
        }
    }
}
