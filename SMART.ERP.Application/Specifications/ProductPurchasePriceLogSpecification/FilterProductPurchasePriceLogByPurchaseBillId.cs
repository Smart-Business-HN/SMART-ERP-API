using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.ProductPurchasePriceLogSpecification
{
    public class FilterProductPurchasePriceLogByPurchaseBillId : Specification<ProductPurchasePriceLog>
    {
        public FilterProductPurchasePriceLogByPurchaseBillId(int purchaseBillId)
        {
            Query.Where(x => x.PurchaseBillOriginId == purchaseBillId);
        }
    }
}
