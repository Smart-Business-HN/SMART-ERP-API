using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.ProductToPurchaseSpecification
{
    public class ProductToPurchaseSpecification : Specification<ProductToPurchase>
    {
        public ProductToPurchaseSpecification(int purchaseOrderId) {
            Query.Include(x => x.Tax).Include(x => x.Product).Where(x => x.PurchaseOrderId == purchaseOrderId);
        }
    }
}
