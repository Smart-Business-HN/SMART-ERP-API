using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.OpportunitySpecification
{
    public class GetOpportunityCustomerSpecification : Specification<Opportunity>
    {
        public GetOpportunityCustomerSpecification(int id)
        {
            Query.Include(x => x.Customer).Where(x => x.Id == id).AsNoTracking();
        }
    }
}
