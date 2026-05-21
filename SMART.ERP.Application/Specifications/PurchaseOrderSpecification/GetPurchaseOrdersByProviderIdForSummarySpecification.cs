using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.PurchaseOrderSpecification
{
    public class GetPurchaseOrdersByProviderIdForSummarySpecification : Specification<PurchaseOrder>
    {
        public GetPurchaseOrdersByProviderIdForSummarySpecification(int providerId)
        {
            Query
                .Where(x => x.ProviderId == providerId)
                .Include(x => x.ProductsToPurchase!).ThenInclude(p => p.Product)
                .AsNoTracking();
        }
    }
}
