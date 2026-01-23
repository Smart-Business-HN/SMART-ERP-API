using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.PurchaseBillSpecification
{
    public class FilterPurchaseBillByPendingWithProviderSpecification : Specification<PurchaseBill>
    {
        public FilterPurchaseBillByPendingWithProviderSpecification()
        {
            Query.Include(x => x.Provider)
                .Where(x => x.Outstanding > 0)
                .AsNoTracking();
        }
    }
}
