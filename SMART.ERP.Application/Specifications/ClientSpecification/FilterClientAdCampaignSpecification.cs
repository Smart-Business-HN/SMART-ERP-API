using Ardalis.Specification;
using SMART.MASTER.Domain.Entities;

namespace SMART.ERP.Application.Specifications.ClientSpecification
{
    public class FilterClientAdCampaignSpecification : Specification<Client>
    {
        public FilterClientAdCampaignSpecification(int? headingId, int? customerTypeId)
        {
            if (headingId != null && customerTypeId != null)
            {
                Query.Where(x => x.CustomerTypeId == customerTypeId && x.HeadingId == headingId).AsNoTracking();
            }
            else
            {
                if (headingId == null)
                {
                    Query.Where(x => x.HeadingId == headingId).AsNoTracking();
                }
                else
                {
                    Query.Where(x => x.CustomerTypeId == customerTypeId).AsNoTracking();
                }
            }
        }
    }
}
