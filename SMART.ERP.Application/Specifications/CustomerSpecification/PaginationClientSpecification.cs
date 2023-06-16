using Ardalis.Specification;
using SMART.MASTER.Domain.Entities;

namespace SMART.ERP.Application.Specifications.CustomerSpecification
{
    public class PaginationClientSpecification : Specification<Client>
    {
        public PaginationClientSpecification(string? parameter, int? pageSize, string? order, string? column)
        {
            if (pageSize != null)
            {
                Query.Take((int)pageSize).AsNoTracking();
            }

            if (!string.IsNullOrEmpty(parameter))
            {
                Query.Where(x => x.FullName.Contains(parameter)
                || x.PhoneNumber!.Contains(parameter)
                || x.Email!.Contains(parameter)
                || x.DNI!.Contains(parameter)
                || x.RTN!.Contains(parameter)).AsNoTracking();
            }

            if (!string.IsNullOrEmpty(order) && !string.IsNullOrEmpty(column))
            {
                if (order == "desc")
                {
                    Query.OrderByDescending(x => column == "FullName" ? x.FullName
                    : column == "PhoneNumber" ? x.PhoneNumber
                    : column == "Email" ? x.Email : null).AsNoTracking();
                }
                else
                {
                    Query.OrderBy(x => column == "FullName" ? x.FullName
                    : column == "PhoneNumber" ? x.PhoneNumber
                    : column == "Email" ? x.Email : null).AsNoTracking();
                }
            }
        }
    }
}
