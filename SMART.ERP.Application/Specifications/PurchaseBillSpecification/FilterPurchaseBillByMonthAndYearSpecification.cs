
using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.PurchaseBillSpecification
{
    public class FilterPurchaseBillByMonthAndYearSpecification : Specification<PurchaseBill>
    {
        public FilterPurchaseBillByMonthAndYearSpecification(int month, int year)
        {
            Query.Include(x => x.Provider).Where(x => x.InvoiceDate.Month == month && x.InvoiceDate.Year == year);
        }
    }
}
