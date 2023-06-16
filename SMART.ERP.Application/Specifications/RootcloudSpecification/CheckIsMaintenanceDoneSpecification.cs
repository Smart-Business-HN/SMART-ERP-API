using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.RootcloudSpecification
{
    public class CheckIsMaintenanceDoneSpecification : Specification<MachineryRootcloudHistorical>
    {
        public CheckIsMaintenanceDoneSpecification(string serialNum)
        {
            //Query.AsNoTracking().Where(a => a.SerialNum == serialNum).OrderByDescending(a => a.Id);
        }
    }
}
