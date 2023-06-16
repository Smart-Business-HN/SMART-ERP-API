using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.MachinerySpecification
{
    public class PaginationMaintenanceSpecification : Specification<MachineryMaintenance>
    {
        public PaginationMaintenanceSpecification(string? parameter, int pageNumber, int pageSize,
            string? order, string? column)
        {
            Query.Include(x => x.Machinery).Skip((pageNumber) * pageSize).Take(pageSize);

            if (!string.IsNullOrEmpty(parameter))
            {
                Query.Where(x => x.Person.Contains(parameter)
                || x.Hourmeter.ToString().Contains(parameter)
                || x.Observation!.Contains(parameter)
                || x.CreatedBy.Contains(parameter)
                || x.CreationDate.ToString().Contains(parameter)
                || x.Machinery.SerialNum.ToLower().Contains(parameter.ToLower())
                || x.Machinery.Customer.ToLower().Contains(parameter.ToLower()));
            }

            if (!string.IsNullOrEmpty(order) && !string.IsNullOrEmpty(column))
            {
                if (order == "desc")
                {
                    Query.OrderByDescending(x => column == "Person" ? x.Person
                    : column == "Observation" ? x.Observation
                    : column == "CreatedBy" ? x.CreatedBy
                    : column == "CreationDate" ? x.CreationDate
                    : column == "Hourmeter" ? x.Hourmeter
                    : column == "SerialNum" ? x.Machinery.SerialNum
                    : column == "Customer" ? x.Machinery.Customer : null);
                }
                else
                {
                    Query.OrderBy(x => column == "Person" ? x.Person
                    : column == "Observation" ? x.Observation
                    : column == "CreatedBy" ? x.CreatedBy
                    : column == "CreationDate" ? x.CreationDate
                    : column == "Hourmeter" ? x.Hourmeter
                    : column == "SerialNum" ? x.Machinery.SerialNum
                    : column == "Customer" ? x.Machinery.Customer : null);
                }
            }
        }
    }
}
