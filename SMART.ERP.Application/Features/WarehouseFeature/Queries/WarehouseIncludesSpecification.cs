using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.WarehouseFeature.Queries
{
    public class WarehouseIncludesSpecification : Specification<Warehouse>
    {
        public WarehouseIncludesSpecification(int? id)
        {
            if (id == null)
            {
                Query.Include(a => a.InventoryDistributions).ThenInclude(a => a.Product);
            }
            else
            {
                Query.Include(a=>a.InventoryDistributions).ThenInclude(a=>a.Product).Where(a => a.Id == id);
            }
        }
    }
}