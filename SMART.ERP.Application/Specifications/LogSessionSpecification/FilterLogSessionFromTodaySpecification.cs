using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.LogSessionSpecification
{
    public class FilterLogSessionFromTodaySpecification : Specification<LogSession>
    {
        public FilterLogSessionFromTodaySpecification()
        {
            DateTime today = DateTime.Now;
            Query.Where(x => x.RegisterDate.Date == today.Date && x.User!.Role!.Name == "Sales Advisor");
        }
    }
}
