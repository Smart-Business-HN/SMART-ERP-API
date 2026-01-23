using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.PurchaseBillPaymentSpecification
{
    public class FilterPurchaseBillPaymentByDateRangeSpecification : Specification<PurchaseBillPayment>
    {
        public FilterPurchaseBillPaymentByDateRangeSpecification(DateTime startDate, DateTime endDate)
        {
            Query.Where(x => x.Date >= startDate && x.Date <= endDate).AsNoTracking();
        }
    }
}
