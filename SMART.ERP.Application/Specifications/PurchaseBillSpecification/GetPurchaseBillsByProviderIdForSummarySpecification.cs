using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.PurchaseBillSpecification
{
    public class GetPurchaseBillsByProviderIdForSummarySpecification : Specification<PurchaseBill>
    {
        public GetPurchaseBillsByProviderIdForSummarySpecification(int providerId)
        {
            Query
                .Where(x => x.ProviderId == providerId)
                .Include(x => x.PurchaseBillPayments)
                .AsNoTracking();
        }
    }
}
