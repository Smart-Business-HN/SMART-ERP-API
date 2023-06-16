using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.OpportunityCommentSpecification
{
    public class FilterOpportunityCommentSpecification : Specification<OpportunityComment>
    {
        public FilterOpportunityCommentSpecification(int? id, Guid userId, int opportunityId, DateTime creationDate)
        {
            if (id != null)
            {
                Query.Include(x => x.User).Where(x => x.CreationDate.Date == creationDate.Date
                && x.UserId == userId && x.Id != id && x.OpportunityId != opportunityId).AsNoTracking();
            }
            else
            {
                Query.Include(x => x.User).Where(x => x.CreationDate.Date == creationDate.Date
                && x.UserId == userId && x.OpportunityId == opportunityId).AsNoTracking();
            }
        }
    }
}
