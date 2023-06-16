using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.NotificationSpecification
{
    public class FilterUnreadNotificationByUserSpecification : Specification<Notification>
    {
        public FilterUnreadNotificationByUserSpecification(Guid userId)
        {
            Query.Where(x => x.Read == false && x.UserId == userId).OrderByDescending(x => x.Time).AsNoTracking();
        }
    }
}
