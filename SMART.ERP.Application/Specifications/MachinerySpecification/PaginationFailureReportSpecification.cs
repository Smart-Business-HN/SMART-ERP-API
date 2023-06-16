using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.MachinerySpecification
{
    public class PaginationFailureReportSpecification : Specification<MachineryFailureReport>
    {
        public PaginationFailureReportSpecification(string? parameter, int pageNumber, int pageSize,
            string? order, string? column)
        {
            Query.Include(i => i.MachineryFailure).Include(i => i.Status)
                .Include(i => i.Machinery)
                .Where(a => a.MachineryFailure.Name != "Sin falla").Skip((pageNumber) * pageSize).Take(pageSize);

            if (!string.IsNullOrEmpty(parameter))
            {
                Query.Where(x => x.Status!.Name.Contains(parameter)
                || x.Description.Contains(parameter)
                || x.CreatedBy.Contains(parameter)
                || x.Machinery.SerialNum.ToLower().Contains(parameter.ToLower())
                || x.Machinery.Customer.ToLower().Contains(parameter.ToLower())
                || x.MachineryFailure!.Name.Contains(parameter));
            }

            if (!string.IsNullOrEmpty(order) && !string.IsNullOrEmpty(column))
            {
                if (order == "desc")
                {
                    Query.OrderByDescending(x => column == "Status" ? x.Status!.Name
                    : column == "MachineryFailure" ? x.MachineryFailure!.Name
                    : column == "CreatedBy" ? x.CreatedBy
                    : column == "Description" ? x.Description
                    : column == "CreationDate" ? x.CreationDate
                    : column == "SerialNum" ? x.Machinery.SerialNum
                    : column == "Customer" ? x.Machinery.Customer : null);
                }
                else
                {
                    Query.OrderBy(x => column == "Status" ? x.Status!.Name
                    : column == "MachineryFailure" ? x.MachineryFailure!.Name
                    : column == "CreatedBy" ? x.CreatedBy
                    : column == "Description" ? x.Description
                    : column == "CreationDate" ? x.CreationDate
                    : column == "SerialNum" ? x.Machinery.SerialNum
                    : column == "Customer" ? x.Machinery.Customer : null);
                }
            }
        }
    }
}
