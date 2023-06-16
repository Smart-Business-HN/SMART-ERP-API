using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.ProspectSpecification
{
    public class MasterProspectSpecification : Specification<Prospect>
    {
        public MasterProspectSpecification(Guid? UserId, int? prospectStep)
        {
            Query.Include(x => x.User).Include(x => x.ProspectStep).Include(x => x.ProspectQuoteProducts);
            if (UserId != null)
            {
                Query.Where(x => x.UserId == UserId);
            }
            if (prospectStep != null)
            {
                Query.Where(x => x.ProspectStepId == prospectStep);
            }

        }
    }
}
