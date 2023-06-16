using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.OpportunityActivitySpecification
{
    public class OpportunityActivityIncludesSpecification : Specification<OpportunityActivity>
    {
        public OpportunityActivityIncludesSpecification(int? id)
        {
            if (id != null)
            {
                Query.Include(x => x.TypeActivity).Include(x => x.Status).Include(x => x.User).Where(x => x.Id == id).AsNoTracking();
            }
            else
            {
                Query.Include(x => x.TypeActivity).Include(x => x.Status).Include(x => x.User).AsNoTracking();
            }
        }
    }
}
