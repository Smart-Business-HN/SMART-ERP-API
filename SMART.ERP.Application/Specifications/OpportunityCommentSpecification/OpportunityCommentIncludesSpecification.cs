using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.OpportunityCommentSpecification
{
    public class OpportunityCommentIncludesSpecification : Specification<OpportunityComment>
    {
        public OpportunityCommentIncludesSpecification(int? id)
        {
            if (id != null)
            {
                Query.Include(x => x.User).Where(x => x.Id == id).AsNoTracking();
            }
            else
            {
                Query.Include(x => x.User).AsNoTracking();
            }
        }
    }
}
