using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.InventoryInputSpecification
{
    public class InventoryInputIncludesSpecification : Specification<InventoryInput>
    {
        public InventoryInputIncludesSpecification(int id)
        {
            Query.Include(x => x.Prefix).Include(x => x.ProductEntries).ThenInclude(x => x.Product).Include(x => x.Status).Include(x => x.Warehouse).Include(x => x.InventoryInputType).Where(x => x.Id == id);
        }
    }
}
