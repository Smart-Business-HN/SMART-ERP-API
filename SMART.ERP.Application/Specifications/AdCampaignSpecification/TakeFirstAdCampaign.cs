using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.AdCampaignSpecification
{
    public class TakeFirstAdCampaign : Specification<MetaAdCampaign>
    {
        public TakeFirstAdCampaign()
        {
            Query.Take(1);
        }
    }
}
