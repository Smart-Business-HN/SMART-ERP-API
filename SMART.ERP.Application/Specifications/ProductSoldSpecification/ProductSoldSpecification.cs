using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.ProductSoldSpecification
{
    public class ProductSoldSpecification : Specification<ProductSold>
    {
        public ProductSoldSpecification(int invoiceId)
        {
            // IgnoreQueryFilters: historico, debe resolver productos eliminados (soft delete).
            Query.IgnoreQueryFilters();
            Query.Include(x => x.Product).Include(x=>x.Tax).Where(x=>x.InvoiceId == invoiceId);
        }
    }
}
