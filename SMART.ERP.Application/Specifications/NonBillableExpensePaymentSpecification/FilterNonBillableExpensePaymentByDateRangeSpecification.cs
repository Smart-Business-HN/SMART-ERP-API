using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.NonBillableExpensePaymentSpecification
{
    public class FilterNonBillableExpensePaymentByDateRangeSpecification : Specification<NonBillableExpensePayment>
    {
        public FilterNonBillableExpensePaymentByDateRangeSpecification(DateTime startDate, DateTime endDate)
        {
            Query.Where(x => x.Date >= startDate && x.Date <= endDate).AsNoTracking();
        }
    }
}
