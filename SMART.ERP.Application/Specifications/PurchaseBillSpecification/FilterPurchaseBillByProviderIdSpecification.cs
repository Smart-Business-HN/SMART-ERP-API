using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.PurchaseBillSpecification
{
    public class FilterPurchaseBillByProviderIdSpecification : Specification<PurchaseBill>
    {
        public FilterPurchaseBillByProviderIdSpecification(int providerId, string? parameter, int pageNumber, int pageSize)
        {
            Query
                .Include(x => x.Status)
                .Where(x => x.ProviderId == providerId)
                .OrderByDescending(x => x.InvoiceDate)
                .AsNoTracking();

            if (!string.IsNullOrEmpty(parameter))
            {
                Query.Where(x => x.PurchaseBillCode.Contains(parameter) || x.InvoiceNumber.Contains(parameter));
            }

            Query.Skip(pageNumber * pageSize).Take(pageSize);
        }
    }

    public class CountPurchaseBillByProviderIdSpecification : Specification<PurchaseBill>
    {
        public CountPurchaseBillByProviderIdSpecification(int providerId, string? parameter)
        {
            Query
                .Where(x => x.ProviderId == providerId)
                .AsNoTracking();

            if (!string.IsNullOrEmpty(parameter))
            {
                Query.Where(x => x.PurchaseBillCode.Contains(parameter) || x.InvoiceNumber.Contains(parameter));
            }
        }
    }
}
