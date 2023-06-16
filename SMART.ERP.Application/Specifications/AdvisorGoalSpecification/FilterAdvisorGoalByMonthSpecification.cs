using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.AdvisorGoalSpecification
{
    public class FilterAdvisorGoalByMonthSpecification : Specification<AdvisorGoal>
    {
        public FilterAdvisorGoalByMonthSpecification(Guid? id, int? branchId, DateTime date)
        {
            Query.Include(x => x.User).Where(x => x.InitDate.Month == date.Month && x.InitDate.Year == date.Year).AsNoTracking();
            if (id != null)
            {
                Query.Where(x => x.UserId == id);
            }
            if (branchId != null)
            {
                Query.Where(x => x.User!.BranchOfficeId == branchId);
            }
        }
    }
}
