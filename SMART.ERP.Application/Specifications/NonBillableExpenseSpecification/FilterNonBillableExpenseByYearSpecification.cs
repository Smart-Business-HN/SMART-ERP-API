using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.NonBillableExpenseSpecification;

public sealed class FilterNonBillableExpenseByYearSpecification : Specification<NonBillableExpense>
{
    public FilterNonBillableExpenseByYearSpecification(DateTime date)
    {
        Query.Include(x => x.Provider);
        Query.Include(x => x.ExpenseAccount);
        Query.Where(x => x.Date.Year == date.Year && !x.Provider!.Name.Contains("Banco"));
    }
}