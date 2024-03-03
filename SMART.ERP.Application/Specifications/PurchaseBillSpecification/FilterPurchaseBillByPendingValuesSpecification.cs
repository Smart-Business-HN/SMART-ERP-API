using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.PurchaseBillSpecification
{
    public class FilterPurchaseBillByPendingValuesSpecification : Specification<PurchaseBill>
    {
        public FilterPurchaseBillByPendingValuesSpecification() {
            Query.Where(x => x.Outstanding > 0);
        }
    }
}
