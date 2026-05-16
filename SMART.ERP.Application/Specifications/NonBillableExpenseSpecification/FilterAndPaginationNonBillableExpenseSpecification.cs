using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.NonBillableExpenseSpecification
{
    public class FilterAndPaginationNonBillableExpenseSpecification : Specification<NonBillableExpense>
    {
        public FilterAndPaginationNonBillableExpenseSpecification(string? parameter, int pageNumber, int pageSize,
            string? order, string? column)
        {
            Query.Include(x => x.Provider).Include(x => x.Status).Where(x => x.LegacyMigratedToInternalBankAccountId == null).Skip((pageNumber) * pageSize).OrderByDescending(x => x.Id).Take(pageSize).AsNoTracking();

            if (!string.IsNullOrEmpty(parameter))
            {
                Query.Where(x => x.Description.Contains(parameter) || x.Provider!.Name.Contains(parameter));
            }

            if (!string.IsNullOrEmpty(order) && !string.IsNullOrEmpty(column))
            {
                if (order == "desc")
                {
                    Query.OrderByDescending(x => column == "Name" ? x.Description : null);
                }
                else
                {
                    Query.OrderBy(x => column == "Name" ? x.Description : null);
                }
            }
        }
    }
}
