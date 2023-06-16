using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.OpportunitySpecification
{
    public class OpportunityIncludesSpecification : Specification<Opportunity>
    {
        public OpportunityIncludesSpecification(int? id, Guid? customerId)
        {
            Query.Include(x => x.QuoteProducts!.Where(x => x.IsActive)).ThenInclude(x => x.Product).ThenInclude(x => x!.Brand)
                .Include(x => x.QuoteProducts!.Where(x => x.IsActive)).ThenInclude(x => x.Product)
                .Include(x => x.QuoteProducts!.Where(x => x.IsActive)).ThenInclude(x => x.Product).ThenInclude(x => x!.SubCategory)
                .Include(x => x.QuoteProducts!.Where(x => x.IsActive)).ThenInclude(x => x.Product).ThenInclude(x => x!.ProductImages)
                .Include(x => x.OpportunityActivities!).ThenInclude(x => x.Status).Include(x => x.OpportunityActivities!).ThenInclude(x => x.User)
                .Include(x => x.OpportunityActivities!.OrderBy(y => y.StatusId)).ThenInclude(x => x.TypeActivity)
                .Include(x => x.OpportunityComments!).ThenInclude(x => x.User)
                .Include(x => x.OpportunityDocuments!).ThenInclude(x => x.DocumentType).Include(x => x.OpportunityDocuments!).ThenInclude(x => x.User)
                .Include(x => x.Customer).Include(x => x.User).Include(x => x.InterestLevel).Include(x => x.OpportunityStep).Include(x => x.WinReason)
                .Include(x => x.LossReason).Include(x => x.TypeOrigin).OrderBy(x => x.Position);

            if (id != null)
            {
                Query.Where(x => x.Id == id).Include(x => x.LossReason);

            }
            else if (customerId != null)
            {
                Query.Where(x => x.CustomerId == customerId && x.IsActive);

            }
        }
    }
}
