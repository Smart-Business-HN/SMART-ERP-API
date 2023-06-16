using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.MachinerySpecification
{
    public class GetMachineriesSystemRunningSpecification : Specification<Machinery>
    {
        public GetMachineriesSystemRunningSpecification(int? subcategoryId, string? country, int? brandId, string? status)
        {
            if (subcategoryId > 0 && subcategoryId != null && country != null)
                Query.Where(a => a.SubcategoryId == subcategoryId && a.Country == country)
                    .Include(i => i.MachineyRootcloudHistoricals!.OrderByDescending(i => i.Id).Take(1))
                    .Include(i => i.MachineryFailureReports!.OrderByDescending(i => i.Id).Take(1))
                    .ThenInclude(i => i.MachineryFailure);
            else if (country != null)
                Query.Where(x => x.Country == country)
                    .Include(i => i.MachineyRootcloudHistoricals!.OrderByDescending(i => i.Id).Take(1))
                    .Include(i => i.MachineryFailureReports!.OrderByDescending(i => i.Id).Take(1))
                    .ThenInclude(i => i.MachineryFailure);
            else
                Query.Include(i => i.MachineyRootcloudHistoricals!.OrderByDescending(i => i.Id).Take(1))
                    .Include(i => i.MachineryFailureReports!.OrderByDescending(i => i.Id).Take(1))
                    .ThenInclude(i => i.MachineryFailure);

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
