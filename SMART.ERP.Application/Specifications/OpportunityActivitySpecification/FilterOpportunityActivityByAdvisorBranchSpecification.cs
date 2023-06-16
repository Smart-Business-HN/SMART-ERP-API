using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.OpportunityActivitySpecification
{
    public class FilterOpportunityActivityByAdvisorBranchSpecification : Specification<OpportunityActivity>
    {
        public FilterOpportunityActivityByAdvisorBranchSpecification(Guid? userId, int? branchOfficeId, DateTime initDate, DateTime endDate)
        {
            if (userId != null)
            {
                Query.Include(a => a.User).Include(a => a.Status).Include(a => a.TypeActivity)
                    .Where(x => x.UserId == userId && x.CreationDate >= initDate && x.EndDate <= endDate).AsNoTracking();
            }
            else if (userId == null && branchOfficeId != null && branchOfficeId > 0)
            {
                Query.Include(a => a.User).Include(a => a.Status).Include(a => a.TypeActivity)
                    .Where(x => x.User!.BranchOfficeId == branchOfficeId && x.CreationDate >= initDate && x.EndDate <= endDate).AsNoTracking();
            }
            else if (userId != null && branchOfficeId != null && branchOfficeId > 0)
            {
                Query.Include(a => a.User).Include(a => a.Status).Include(a => a.TypeActivity)
                    .Where(x => x.UserId == userId && x.User!.BranchOfficeId == branchOfficeId
                    && x.CreationDate >= initDate && x.EndDate <= endDate).AsNoTracking();
            }
            else
            {
                Query.Include(a => a.User).Include(a => a.Status).Include(a => a.TypeActivity)
                    .Where(x => x.CreationDate >= initDate && x.EndDate <= endDate).AsNoTracking();
            }
        }
    }
}
