using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.ClientSpecification
{
    public class FilterClientByIdSpecification : Specification<Customer>
    {
        public FilterClientByIdSpecification(Guid? id)
        {
            if (id != null)
            {
                Query
                    .Include(x => x.Gender)
                    .Include(x => x.SocialReason)
                    .Include(x => x.Heading)
                    .Include(x => x.CustomerType)
                    .Include(x => x.PriceList)
                    .Include(x => x.DeliveryDirections!).ThenInclude(x => x.City)
                    .Where(a => a.Id == id).AsNoTracking();
            }
            else
            {
                Query
                    .Include(x => x.Gender)
                    .Include(x => x.Department)
                    .Include(x => x.SocialReason)
                    .Include(x => x.CustomerType)
                    .Include(x => x.PriceList)
                    .Include(x => x.Heading)
                    .Include(x => x.DeliveryDirections!).ThenInclude(x => x.City).AsNoTracking();
            }
        }
    }
}
