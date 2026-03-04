using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.ChatSpecification
{
    public class FilterChatSessionByIdentifierSpecification : Specification<ChatSession>
    {
        public FilterChatSessionByIdentifierSpecification(string sessionIdentifier)
        {
            Query.Where(x => x.SessionIdentifier == sessionIdentifier && x.Status != 2)
                .Include(x => x.AssignedAdminUser)
                .AsNoTracking();
        }
    }
}
