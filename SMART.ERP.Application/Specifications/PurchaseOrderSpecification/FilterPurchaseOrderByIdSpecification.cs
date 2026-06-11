using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.PurchaseOrderSpecification
{
    public class FilterPurchaseOrderByIdSpecification : Specification<PurchaseOrder>
    {
        public FilterPurchaseOrderByIdSpecification(int id)
        {
            // IgnoreQueryFilters: historico, debe resolver productos eliminados (soft delete).
            Query.IgnoreQueryFilters();
            Query.Include(x => x.Status)
                 .Include(x => x.Provider)
                 .Include(x => x.Prefix).ThenInclude(x => x!.InternalDocument)
                 .Include(x => x.User)
                 .Include(x => x.ProductsToPurchase)
                 .Include(x => x.ProductsToPurchase)!.ThenInclude(x => x.Product).ThenInclude(x => x!.Brand)
                 .Include(x => x.ProductsToPurchase)!.ThenInclude(x => x.Tax)
                 .Include(x => x.BranchOffice)
                 .Where(x => x.Id == id).AsNoTracking();
        }
    }
}
