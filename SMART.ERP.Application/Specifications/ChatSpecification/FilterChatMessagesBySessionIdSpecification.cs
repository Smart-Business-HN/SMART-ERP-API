using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.ChatSpecification
{
    public class FilterChatMessagesBySessionIdSpecification : Specification<ChatMessage>
    {
        public FilterChatMessagesBySessionIdSpecification(int chatSessionId)
        {
            Query.Where(x => x.ChatSessionId == chatSessionId)
                .OrderBy(x => x.SentAt)
                .AsNoTracking();
        }
    }
}
