using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.NonBillableExpenseSpecification
{
    public class FilterNonBillableExpenseByPendingWithProviderSpecification : Specification<NonBillableExpense>
    {
        public FilterNonBillableExpenseByPendingWithProviderSpecification()
        {
            Query.Include(x => x.Provider)
                .Where(x => x.Outstanding > 0)
                .AsNoTracking();
        }
    }
}
