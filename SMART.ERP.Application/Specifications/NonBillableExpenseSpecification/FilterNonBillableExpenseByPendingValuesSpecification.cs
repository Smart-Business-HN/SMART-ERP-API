using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.NonBillableExpenseSpecification;

public sealed class FilterNonBillableExpenseByPendingValuesSpecification : Specification<NonBillableExpense>
{
    public FilterNonBillableExpenseByPendingValuesSpecification()
    {
        Query.Where(x => x.Outstanding > 0 && x.LegacyMigratedToInternalBankAccountId == null);
    }
}