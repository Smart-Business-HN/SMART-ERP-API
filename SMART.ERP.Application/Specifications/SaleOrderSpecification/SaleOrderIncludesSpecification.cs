using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.SaleOrderSpecification
{
    public class SaleOrderIncludesSpecification : Specification<SaleOrder>
    {
        public SaleOrderIncludesSpecification(int? id)
        {
            // IgnoreQueryFilters: las lineas de orden deben seguir mostrando productos eliminados (historico).
            if (id != null)
            {
                Query.IgnoreQueryFilters().Include(x => x.SaleOrderProducts!).ThenInclude(x => x.Product).Include(x => x.Status)
                    .Include(x => x.FinancingPlan).Include(x => x.Customer).Where(x => x.Id == id);
            }
            else
            {
                Query.IgnoreQueryFilters().Include(x => x.SaleOrderProducts!).ThenInclude(x => x.Product).Include(x => x.Status)
                    .Include(x => x.FinancingPlan).Include(x => x.Customer);
            }
        }
    }
}
