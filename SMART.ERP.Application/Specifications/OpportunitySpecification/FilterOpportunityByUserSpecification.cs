using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.OpportunitySpecification
{
    public class FilterOpportunityByUserSpecification : Specification<Opportunity>
    {
        public FilterOpportunityByUserSpecification(Guid id, bool onlyActive)
        {
            if (onlyActive == true)
            {
                Query.Include(x => x.Customer).Where(x => x.UserId == id && x.OpportunityStep!.Name != "Ganado" && x.OpportunityStep.Name != "Perdido"
                    && x.OpportunityStep.Name != "Abandonado").AsNoTracking();
            }
            else
            {
                Query.Include(x => x.Customer).Where(x => x.UserId == id).AsNoTracking();
            }

        }
    }
}
