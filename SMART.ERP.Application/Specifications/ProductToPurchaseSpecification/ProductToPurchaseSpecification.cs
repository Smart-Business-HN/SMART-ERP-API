using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.ProductToPurchaseSpecification
{
    public class ProductToPurchaseSpecification : Specification<ProductToPurchase>
    {
        public ProductToPurchaseSpecification(int purchaseOrderId) {
            // IgnoreQueryFilters: historico, debe resolver productos eliminados (soft delete).
            Query.IgnoreQueryFilters();
            Query.Include(x => x.Tax).Include(x => x.Product).Where(x => x.PurchaseOrderId == purchaseOrderId);
        }
    }
}
