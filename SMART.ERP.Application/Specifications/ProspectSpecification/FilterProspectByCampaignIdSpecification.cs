using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.ProspectSpecification
{
    public class FilterProspectByCampaignIdSpecification : Specification<Prospect>
    {
        public FilterProspectByCampaignIdSpecification(string campaignId, bool converted)
        {
            Query.Include(x => x.ProspectStep).Where(x => x.MetaAdCampaignId == campaignId);
            if (converted)
            {
                Query.Where(x => x.ProspectStep!.Name == "Convertido");
            }
        }
    }
}
