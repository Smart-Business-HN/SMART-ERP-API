using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.WarehouseSpecification
{
    public class WarehoseIncludesSpecification : Specification<Warehouse>
    {
        public WarehoseIncludesSpecification(int? id)
        {
            // IgnoreQueryFilters: historico, debe resolver productos eliminados (soft delete).
            Query.IgnoreQueryFilters();
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
