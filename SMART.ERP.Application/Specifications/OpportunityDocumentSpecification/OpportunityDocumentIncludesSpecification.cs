using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.OpportunityDocumentSpecification
{
    public class OpportunityDocumentIncludesSpecification : Specification<OpportunityDocument>
    {
        public OpportunityDocumentIncludesSpecification(int? id)
        {
            if (id != null)
            {
                Query.Include(x => x.User).Include(x => x.DocumentType).Where(x => x.Id == id).AsNoTracking();
            }
            else
            {
                Query.Include(x => x.User).Include(x => x.DocumentType).AsNoTracking();
            }
        }
    }
}
