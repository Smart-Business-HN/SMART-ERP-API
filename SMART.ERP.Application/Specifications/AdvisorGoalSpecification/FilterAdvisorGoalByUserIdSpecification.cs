using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.AdvisorGoalSpecification
{
    public class FilterAdvisorGoalByUserIdSpecification : Specification<AdvisorGoal>
    {
        public FilterAdvisorGoalByUserIdSpecification(Guid userId)
        {
            Query.Where(x => x.UserId == userId && x.InitDate.Month == DateTime.Now.Month).AsNoTracking();
        }
    }
}
