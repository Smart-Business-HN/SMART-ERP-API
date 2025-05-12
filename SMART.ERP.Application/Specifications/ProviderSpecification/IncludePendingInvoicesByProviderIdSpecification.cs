using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.ProviderSpecification
{
    public sealed class IncludePendingInvoicesByProviderIdSpecification : Specification<Provider>
    {
        public IncludePendingInvoicesByProviderIdSpecification(int providerId)
        {
            Query.Where(x => x.Id == providerId);
            Query.Include(x => x.PurchaseBills!.Where(y => y.Outstanding > 0)).ThenInclude(z=>z.ExpenseAccount);
            Query.Include(x => x.NonBillableExpenses!.Where(y => y.Outstanding > 0));
            Query.Where(x => x.PurchaseBills!.Any(y => y.Outstanding > 0) || x.NonBillableExpenses!.Any(y => y.Outstanding > 0));
        }
    }
}