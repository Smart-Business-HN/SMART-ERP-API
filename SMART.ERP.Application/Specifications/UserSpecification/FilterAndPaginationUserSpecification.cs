using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.UserSpecification
{
    public class FilterAndPaginationUserSpecification : Specification<User>
    {
        public FilterAndPaginationUserSpecification(string? parameter, int pageNumber, int pageSize,
            string? order, string? column)
        {
            Query.Include(x => x.Gender).Include(x => x.Role)
                .Include(x => x.BranchOffice).Skip((pageNumber) * pageSize).Take(pageSize).AsNoTracking();

            if (!string.IsNullOrEmpty(parameter))
            {
                Query.Where(x => x.FullName.Contains(parameter) || x.Email.Contains(parameter)
                || x.PhoneNumber.Contains(parameter) || x.Role!.Name.Contains(parameter));
            }

            if (!string.IsNullOrEmpty(order) && !string.IsNullOrEmpty(column))
            {
                if (order == "desc")
                {
                    Query.OrderByDescending(x => column == "FullName" ? x.FullName
                    : column == "PhoneNumber" ? x.PhoneNumber : column == "Email" ? x.Email
                    : column == "Role" ? x.Role!.Name : null);
                }
                else
                {
                    Query.OrderBy(x => column == "FullName" ? x.FullName
                    : column == "PhoneNumber" ? x.PhoneNumber : column == "Email" ? x.Email
                    : column == "Role" ? x.Role!.Name : null);
                }
            }
        }
    }
}
