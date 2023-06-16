using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.ReportSpecification
{
    public class AdvisorActivitiesSpecification : Specification<OpportunityActivity>
    {
        public AdvisorActivitiesSpecification(DateTime? start, DateTime? end, int? branchId)
        {
            Query.Include(x => x.Status).AsNoTracking();
            if (start != null)
            {
                Query.Where(x => x.CreationDate >= start);
            }
            if (end != null)
            {
                Query.Where(x => x.CreationDate <= end);
            }
            if (branchId != null)
            {
                Query.Where(x => x.User!.BranchOfficeId == branchId);
            }
        }
    }
}
