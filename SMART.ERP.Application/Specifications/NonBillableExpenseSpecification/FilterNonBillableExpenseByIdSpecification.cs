using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.NonBillableExpenseSpecification
{
    public class FilterNonBillableExpenseByIdSpecification : Specification<NonBillableExpense>
    {
        public FilterNonBillableExpenseByIdSpecification(int id)
        {
            Query
                .Include(x => x.Provider)
                .Include(x => x.ExpenseAccount)
                .Include(x => x.StatusId)
                .Include(x => x.Prefix)
                .Where(x => x.Id == id);
        }
    }
}
