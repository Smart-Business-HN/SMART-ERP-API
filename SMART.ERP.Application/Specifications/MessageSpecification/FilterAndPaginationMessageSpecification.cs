using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.MessageSpecification
{
    public class FilterAndPaginationMessageSpecification : Specification<Message>
    {
        public FilterAndPaginationMessageSpecification(string? parameter, string? order, string? column)
        {
            Query.Include(x => x.Department).Include(x => x.Country).AsNoTracking();
            if (string.IsNullOrEmpty(column))
            {
                Query.OrderByDescending(x => x.Date);
            }

            if (!string.IsNullOrEmpty(parameter))
            {
                Query.Where(x => x.FullName.Contains(parameter) || x.Email.Contains(parameter)
                || x.PhoneNumber.Contains(parameter) || x.Subject.Contains(parameter));
            }

            if (!string.IsNullOrEmpty(order) && !string.IsNullOrEmpty(column))
            {
                if (order == "desc")
                {
                    Query.OrderByDescending(x => column == "FullName" ? x.FullName
                    : column == "PhoneNumber" ? x.PhoneNumber : column == "Email" ? x.Email
                    : column == "Subject" ? x.Subject : column == "Department" ? x.Department!.Name : null);
                }
                else
                {
                    Query.OrderBy(x => column == "FullName" ? x.FullName
                    : column == "PhoneNumber" ? x.PhoneNumber : column == "Email" ? x.Email
                    : column == "Subject" ? x.Subject : column == "Department" ? x.Department!.Name : null);
                }
            }
        }
    }
}
