using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.MachinerySpecification
{
    public class GetMachineriesByStatusSpecification : Specification<Machinery>
    {
        public GetMachineriesByStatusSpecification(int? subcategoryId, string? country, int? brandId, string? status)
        {
            if (subcategoryId > 0 && country != null)
            {
                Query.Include(i => i.MachineyRootcloudHistoricals!.OrderByDescending(i => i.Id).Take(1))
                .Include(i => i.MachineryFailureReports.OrderByDescending(a => a.Id).Take(1)).ThenInclude(ti => ti.Status)
                .Include(i => i.MachineryFailureReports.OrderByDescending(a => a.Id).Take(1)).ThenInclude(ti => ti.MachineryFailure)
                .Where(a => a.SubcategoryId == subcategoryId && a.Country == country).AsNoTracking();
            }
            else if (subcategoryId > 0 && country == null)
            {
                Query.Include(i => i.MachineyRootcloudHistoricals!.OrderByDescending(i => i.Id).Take(1))
                .Include(i => i.MachineryFailureReports.OrderByDescending(a => a.Id).Take(1)).ThenInclude(ti => ti.Status)
                .Include(i => i.MachineryFailureReports.OrderByDescending(a => a.Id).Take(1)).ThenInclude(ti => ti.MachineryFailure)
                .Where(a => a.SubcategoryId == subcategoryId).AsNoTracking();
            }
            else if (country != null && subcategoryId == 0)
            {
                Query.Include(i => i.MachineyRootcloudHistoricals!.OrderByDescending(i => i.Id).Take(1))
                .Include(i => i.MachineryFailureReports.OrderByDescending(a => a.Id).Take(1)).ThenInclude(ti => ti.Status)
                .Include(i => i.MachineryFailureReports.OrderByDescending(a => a.Id).Take(1)).ThenInclude(ti => ti.MachineryFailure)
                .Where(a => a.Country == country).AsNoTracking();
            }
            else
            {
                Query.Include(i => i.MachineyRootcloudHistoricals!.OrderByDescending(i => i.Id).Take(1))
                .Include(i => i.MachineryFailureReports.OrderByDescending(a => a.Id).Take(1)).ThenInclude(ti => ti.Status)
                .Include(i => i.MachineryFailureReports.OrderByDescending(a => a.Id).Take(1)).ThenInclude(ti => ti.MachineryFailure);
            }

            if (brandId > 0 && brandId != null)
            {
                Query.Where(x => x.BrandId == brandId);
            }
            if (status != null)
            {
                Query.Where(x => x.Status == status);
            }
        }
    }
}
