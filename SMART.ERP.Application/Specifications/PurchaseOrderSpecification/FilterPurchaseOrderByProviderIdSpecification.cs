using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.PurchaseOrderSpecification
{
    public class FilterPurchaseOrderByProviderIdSpecification : Specification<PurchaseOrder>
    {
        public FilterPurchaseOrderByProviderIdSpecification(int providerId, string? parameter, int pageNumber, int pageSize)
        {
            Query
                .Include(x => x.Status)
                .Where(x => x.ProviderId == providerId)
                .OrderByDescending(x => x.CreationDate)
                .AsNoTracking();

            if (!string.IsNullOrEmpty(parameter))
            {
                Query.Where(x => x.PurchaseOrderCode.Contains(parameter));
            }

            Query.Skip(pageNumber * pageSize).Take(pageSize);
        }
    }

    public class CountPurchaseOrderByProviderIdSpecification : Specification<PurchaseOrder>
    {
        public CountPurchaseOrderByProviderIdSpecification(int providerId, string? parameter)
        {
            Query
                .Where(x => x.ProviderId == providerId)
                .AsNoTracking();

            if (!string.IsNullOrEmpty(parameter))
            {
                Query.Where(x => x.PurchaseOrderCode.Contains(parameter));
            }
        }
    }
}
