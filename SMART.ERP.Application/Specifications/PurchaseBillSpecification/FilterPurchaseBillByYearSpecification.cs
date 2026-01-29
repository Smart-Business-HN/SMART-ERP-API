using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.PurchaseBillSpecification
{
    public class FilterPurchaseBillByYearSpecification : Specification<PurchaseBill>
    {
        public FilterPurchaseBillByYearSpecification(DateTime date)
        {
            Query.Include(x=>x.ExpenseAccount).Where(x => x.CreationDate.Year == date.Year);
        }
    }
}
