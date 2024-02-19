using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.ExpenseAccountSpecification
{
    public class FilterAndPaginationExpenseAccountSpecification : Specification<ExpenseAccount>
    {
        public FilterAndPaginationExpenseAccountSpecification(string? parameter, int pageNumber,
            int pageSize, string? order, string? column)
        {
            Query.Skip((pageNumber) * pageSize).Take(pageSize).AsNoTracking();

            if (!string.IsNullOrEmpty(parameter))
            {
                Query.Where(x => x.Name.Contains(parameter) || x.AccountNumber.Contains(parameter));
            }

            if (!string.IsNullOrEmpty(order) && !string.IsNullOrEmpty(column))
            {
                if (order == "desc")
                {
                    Query.OrderByDescending(x => x.Name);
                }
                else
                {
                    Query.OrderBy(x => x.Name);
                }
            }
        }
    }
}
