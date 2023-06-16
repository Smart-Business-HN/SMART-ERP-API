using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.QuoteProductSpecification
{
    public class QuoteProductIncludesSpecification : Specification<QuoteProduct>
    {
        public QuoteProductIncludesSpecification(int? id)
        {
            if (id != null)
            {
                Query.Include(x => x.Product).Where(x => x.Id == id && x.IsActive).AsNoTracking();
            }
            else
            {
                Query.Include(x => x.Product).Where(x => x.IsActive).AsNoTracking();
            }
        }
    }
}
