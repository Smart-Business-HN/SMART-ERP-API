using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.MachinerySpecification
{
    public class PaginationMachinerySpecification : Specification<Machinery>
    {
        public PaginationMachinerySpecification(string? parameter, int pageNumber, int pageSize,
            string? order, string? column)
        {
            Query.Include(i => i.Subcategory).Include(x => x.Brand).Include(a => a.MachineyRootcloudHistoricals!.OrderByDescending(a => a.Id).Take(1))
                .Include(i => i.MachineryMaintenances.OrderByDescending(a => a.Id).Take(1))
                .Include(i => i.MachineryFailureReports.OrderByDescending(a => a.Id).Take(1)).ThenInclude(ti => ti.Status!)
                .Include(i => i.MachineryFailureReports.OrderByDescending(a => a.Id).Take(1)).ThenInclude(ti => ti.MachineryFailure!)
                .Skip(pageNumber * pageSize).Take(pageSize);

            if (!string.IsNullOrEmpty(parameter) && parameter != "null")
            {
                Query.Where(x => x.SerialNum.Contains(parameter)
                || x.DeviceName.Contains(parameter)
                || x.Customer!.Contains(parameter)
                || x.CatName.Contains(parameter)
                || x.MachineryFailureReports.OrderByDescending(a => a.Id).FirstOrDefault()!.Status.Name.Contains(parameter)
                || x.MachineryFailureReports.OrderByDescending(a => a.Id).FirstOrDefault()!.MachineryFailure.Name.Contains(parameter)
                || x.MachineyRootcloudHistoricals.OrderByDescending(a => a.Id).FirstOrDefault()!.Hourmeter.ToString().Contains(parameter)
                || x.MachineyRootcloudHistoricals.OrderByDescending(a => a.Id).FirstOrDefault()!.Milenage.ToString().Contains(parameter)
                || x.MachineyRootcloudHistoricals.OrderByDescending(a => a.Id).FirstOrDefault()!.NextMaintenance.ToString().Contains(parameter)
                || x.MachineyRootcloudHistoricals.OrderByDescending(a => a.Id).FirstOrDefault()!.TimestampLocal.Contains(parameter)
                || x.MachineyRootcloudHistoricals.OrderByDescending(a => a.Id).FirstOrDefault()!.MissingForNextMaintenance.ToString().Contains(parameter));
            }

            if (!string.IsNullOrEmpty(order) && !string.IsNullOrEmpty(column))
            {
                if (order == "desc")
                {
                    Query.OrderBy(x => column == "SerialNum" ? x.SerialNum
                    : column == "Customer" ? x.Customer
                    : column == "Status" ? x.MachineryFailureReports.OrderByDescending(a => a.Id).FirstOrDefault()!.Status.Name
                    : column == "MachineryFailure" ? x.MachineryFailureReports.OrderByDescending(a => a.Id).FirstOrDefault()!.MachineryFailure.Name
                    : column == "Hourmeter" ? x.MachineyRootcloudHistoricals.OrderByDescending(a => a.Id).FirstOrDefault()!.Hourmeter
                    : column == "Milenage" ? x.MachineyRootcloudHistoricals.OrderByDescending(a => a.Id).FirstOrDefault()!.Milenage
                    : column == "MissingForNextMaintenance" ? x.MachineyRootcloudHistoricals.OrderByDescending(a => a.Id).FirstOrDefault()!.MissingForNextMaintenance
                    : column == "NextMaintenance" ? x.MachineyRootcloudHistoricals.OrderByDescending(a => a.Id).FirstOrDefault()!.NextMaintenance
                    : column == "LateHours" ? x.MachineyRootcloudHistoricals.OrderByDescending(a => a.Id).FirstOrDefault()!.LateHours
                    : column == "Timestamp_Local" ? x.MachineyRootcloudHistoricals.OrderByDescending(a => a.Id).FirstOrDefault()!.TimestampLocal : null);
                }
                else
                {
                    Query.OrderByDescending(x => column == "SerialNum" ? x.SerialNum
                    : column == "Customer" ? x.Customer
                    : column == "Status" ? x.MachineryFailureReports.OrderByDescending(a => a.Id).FirstOrDefault()!.Status.Name
                    : column == "MachineryFailure" ? x.MachineryFailureReports.OrderByDescending(a => a.Id).FirstOrDefault()!.MachineryFailure.Name
                    : column == "Hourmeter" ? x.MachineyRootcloudHistoricals.OrderByDescending(a => a.Id).FirstOrDefault()!.Hourmeter
                    : column == "Milenage" ? x.MachineyRootcloudHistoricals.OrderByDescending(a => a.Id).FirstOrDefault()!.Milenage
                    : column == "MissingForNextMaintenance" ? x.MachineyRootcloudHistoricals.OrderByDescending(a => a.Id).FirstOrDefault()!.MissingForNextMaintenance
                    : column == "NextMaintenance" ? x.MachineyRootcloudHistoricals.OrderByDescending(a => a.Id).FirstOrDefault()!.NextMaintenance
                    : column == "LateHours" ? x.MachineyRootcloudHistoricals.OrderByDescending(a => a.Id).FirstOrDefault()!.LateHours
                    : column == "Timestamp_Local" ? x.MachineyRootcloudHistoricals.OrderByDescending(a => a.Id).FirstOrDefault()!.TimestampLocal : null);
                }
            }
        }
    }
}
