using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.ProductEntrySpecification
{
    public class ProductEntrySpecification : Specification<ProductEntry>
    {
        public ProductEntrySpecification(int inventoryInputId)
        {
            Query.Include(x => x.Product).Where(x => x.InventoryInputId == inventoryInputId);
        }
    }
}
