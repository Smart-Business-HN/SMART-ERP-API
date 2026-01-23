using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.BillPaymentSpecification
{
    public class FilterBillPaymentByDateRangeSpecification : Specification<BillPayment>
    {
        public FilterBillPaymentByDateRangeSpecification(DateTime startDate, DateTime endDate)
        {
            Query.Where(x => x.Date >= startDate && x.Date <= endDate).AsNoTracking();
        }
    }
}
