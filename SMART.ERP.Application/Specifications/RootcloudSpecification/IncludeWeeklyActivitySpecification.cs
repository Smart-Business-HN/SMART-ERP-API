using Ardalis.Specification;
using SMART.ERP .Domain.Entities;

namespace SMART.ERP.Application.Specifications.RootcloudSpecification
{
    public class IncludeWeeklyActivitySpecification : Specification<MachineryRootcloudHistorical>
    {
        public IncludeWeeklyActivitySpecification(string? order, string? column, string date, string parameter)
        {
            Query.Where(a => a.CreationDate.Date == Convert.ToDateTime(date).Date);

            //if (!string.IsNullOrEmpty(parameter) && parameter != "null")
            //{
            //    Query.Where(x => x.SerialNum.Contains(parameter)
            //    || x.DeviceName.Contains(parameter)
            //    || x.Customer!.Contains(parameter)
            //    || x.CatName.Contains(parameter)
            //    || x.Hourmeter.ToString().Contains(parameter)
            //    || x.Milenage.ToString().Contains(parameter)
            //    || x.NextMaintenance.ToString().Contains(parameter)
            //    || x.TimestampLocal.Contains(parameter)
            //    || x.MissingForNextMaintenance.ToString().Contains(parameter));
            //}

            //if (!string.IsNullOrEmpty(order) && !string.IsNullOrEmpty(column))
            //{
            //    if (order == "desc")
            //    {
            //        Query.OrderBy(x => column == "SerialNum" ? x.SerialNum
            //        : column == "Customer" ? x.Customer
            //        : column == "Hourmeter" ? x.Hourmeter
            //        : column == "Milenage" ? x.Milenage
            //        : column == "MissingForNextMaintenance" ? x.MissingForNextMaintenance
            //        : column == "NextMaintenance" ? x.NextMaintenance
            //        : column == "LateHours" ? x.LateHours
            //        : column == "Timestamp_Local" ? x.TimestampLocal : null);
            //    }
            //    else
            //    {
            //        Query.OrderByDescending(x => column == "SerialNum" ? x.SerialNum
            //        : column == "Customer" ? x.Customer
            //        : column == "Hourmeter" ? x.Hourmeter
            //        : column == "Milenage" ? x.Milenage
            //        : column == "MissingForNextMaintenance" ? x.MissingForNextMaintenance
            //        : column == "NextMaintenance" ? x.NextMaintenance
            //        : column == "LateHours" ? x.LateHours
            //        : column == "Timestamp_Local" ? x.TimestampLocal : null);
            //    }
            //}
        }
    }
}
