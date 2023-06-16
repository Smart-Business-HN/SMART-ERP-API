using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.RootcloudSpecification
{
    public class FilterByDateWeeklyActivitySpecification : Specification<RootcloudWeeklyActivity>
    {
        public FilterByDateWeeklyActivitySpecification(DateTime date)
        {
            Query.Where(x => x.CreationDate.Date == date.Date);
        }
    }
}
