using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.ProductOfferedSpecification
{
    public class ProductOfferedSpecification : Specification<ProductOffered>
    {
        public ProductOfferedSpecification(int quotationId)
        {
            // IgnoreQueryFilters: historico, debe resolver productos eliminados (soft delete).
            Query.IgnoreQueryFilters();
            Query.Include(x => x.Tax).Include(x=>x.Product).Where(x => x.QuotationId == quotationId);
        }
    }
}
