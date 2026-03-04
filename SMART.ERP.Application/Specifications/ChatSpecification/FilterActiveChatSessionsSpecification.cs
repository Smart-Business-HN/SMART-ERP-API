using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.ChatSpecification
{
    public class FilterActiveChatSessionsSpecification : Specification<ChatSession>
    {
        public FilterActiveChatSessionsSpecification()
        {
            Query.Where(x => x.Status != 2)
                .Include(x => x.AssignedAdminUser)
                .OrderByDescending(x => x.LastMessageAt ?? x.CreatedAt)
                .AsNoTracking();
        }
    }
}
