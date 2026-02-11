using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.EcommerceUserSpecification
{
    public class FilterAndPaginationEcommerceUserSpecification : Specification<EcommerceUser>
    {
        public FilterAndPaginationEcommerceUserSpecification(string? parameter, int pageNumber, int pageSize,
            string? order, string? column, bool? isActive, int? customerTypeId, int? departmentId,
            DateTime? dateFrom, DateTime? dateTo)
        {
            Query.Include(x => x.CustomerType).Include(x => x.Department)
                .Include(x => x.Gender).Skip(pageNumber * pageSize).Take(pageSize).AsNoTracking();

            if (!string.IsNullOrEmpty(parameter))
            {
                Query.Where(x => x.FullName.Contains(parameter) || x.Email.Contains(parameter)
                || x.UserName.Contains(parameter) || x.PhoneNumber.Contains(parameter));
            }

            if (isActive.HasValue)
            {
                Query.Where(x => x.IsActive == isActive.Value);
            }

            if (customerTypeId.HasValue)
            {
                Query.Where(x => x.CustomerTypeId == customerTypeId.Value);
            }

            if (departmentId.HasValue)
            {
                Query.Where(x => x.DepartmentId == departmentId.Value);
            }

            if (dateFrom.HasValue)
            {
                Query.Where(x => x.CreationDate >= dateFrom.Value);
            }

            if (dateTo.HasValue)
            {
                Query.Where(x => x.CreationDate <= dateTo.Value);
            }

            if (!string.IsNullOrEmpty(order) && !string.IsNullOrEmpty(column))
            {
                if (order == "desc")
                {
                    Query.OrderByDescending(x => column == "FullName" ? x.FullName
                    : column == "Email" ? x.Email : column == "UserName" ? x.UserName
                    : column == "PhoneNumber" ? x.PhoneNumber : null);
                }
                else
                {
                    Query.OrderBy(x => column == "FullName" ? x.FullName
                    : column == "Email" ? x.Email : column == "UserName" ? x.UserName
                    : column == "PhoneNumber" ? x.PhoneNumber : null);
                }
            }
        }
    }
}
