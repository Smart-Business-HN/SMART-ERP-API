using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.ProspectSpecification
{
    public class ProspectIncludesSpecification : Specification<Prospect>
    {
        public ProspectIncludesSpecification(Guid? Id)
        {
            // IgnoreQueryFilters: historico, debe resolver productos eliminados (soft delete).
            Query.IgnoreQueryFilters();
            Query.Include(x => x.Department).Include(x => x.ProspectStep).Include(x => x.User).Include(x => x.ProspectQuoteProducts!.Where(x => x.IsActive))
                .ThenInclude(x => x.Product).ThenInclude(x => x!.Brand).Include(x => x.ProspectQuoteProducts!.Where(x => x.IsActive))
                .ThenInclude(x => x.Product).ThenInclude(x => x!.SubCategory).AsNoTracking();
            if (Id != null)
            {
                Query.Where(x => x.Id == Id);
            }
        }
    }
}
