using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.WarehouseSpecification
{
    public class FilterWarehouseByBranchOfficeIdSpecification : Specification<Warehouse>
    {
        public FilterWarehouseByBranchOfficeIdSpecification(int branchOfficeId)
        {
            Query.Include(x => x.InventoryDistributions).Where(x => x.BranchOfficeId == branchOfficeId);
        }
    }
}
