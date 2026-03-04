using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.ProductSpecification
{
    public class FilterProductByIdWithInventorySpec : Specification<Product>
    {
        public FilterProductByIdWithInventorySpec(int productId)
        {
            Query.Where(p => p.Id == productId)
                .Include(p => p.InventoryDistributions!)
                .ThenInclude(d => d.Warehouse)
                .ThenInclude(w => w!.ProviderWarehouses!)
                .ThenInclude(pw => pw.Provider);
        }
    }
}
