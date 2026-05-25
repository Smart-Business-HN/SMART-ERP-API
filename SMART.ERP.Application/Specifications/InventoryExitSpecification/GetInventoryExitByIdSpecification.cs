using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.InventoryExitSpecification
{
    public class GetInventoryExitByIdSpecification : Specification<InventoryExit>
    {
        public GetInventoryExitByIdSpecification(int id)
        {
            Query.Where(x => x.Id == id)
                 .Include(x => x.Warehouse)
                 .Include(x => x.Prefix)
                 .Include(x => x.Items!)
                    .ThenInclude(i => i.Product)
                 .AsNoTracking();
        }
    }
}
