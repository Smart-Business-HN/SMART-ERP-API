using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.WarehouseSpecification
{
    public class WarehoseIncludesSpecification : Specification<Warehouse>
    {
        public WarehoseIncludesSpecification(int? id)
        {
            if (id == null)
            {
                Query.Include(a => a.InventoryDistributions!).ThenInclude(a => a.Product);
            }
            else
            {
                Query.Include(a => a.InventoryDistributions!).ThenInclude(a=>a.Product).Where(a => a.Id == id);
            }
        }
    }
}
