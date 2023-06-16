using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.ProspectQuoteProductSpecification
{
    public class FilterProspectQuoteProductByProspectIdSpecification : Specification<ProspectQuoteProduct>
    {
        public FilterProspectQuoteProductByProspectIdSpecification(Guid prospectId)
        {
            Query.Where(x => x.ProspectId == prospectId && x.IsActive == true);
        }
    }
}
