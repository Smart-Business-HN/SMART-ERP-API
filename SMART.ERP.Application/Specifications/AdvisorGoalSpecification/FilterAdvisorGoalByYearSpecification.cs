using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.AdvisorGoalSpecification
{
    public class FilterAdvisorGoalByYearSpecification : Specification<AdvisorGoal>
    {
        public FilterAdvisorGoalByYearSpecification(int year, Guid? userId)
        {
            Query.Where(x => x.InitDate.Year == year);
            if (userId != null)
            {
                Query.Where(x => x.UserId == userId);
            }
        }
    }
}
