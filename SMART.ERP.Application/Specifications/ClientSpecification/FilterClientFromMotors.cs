using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.ClientSpecification
{
    public class FilterClientFromMotors : Specification<Customer>
    {
        public FilterClientFromMotors(List<Guid> customers)
        {
            Query
                .Include(x => x.Gender)
                .Include(x => x.Department)
                .Include(x => x.SocialReason)
                .Include(x => x.CustomerType)
                .Include(x => x.Heading)
                .Include(x => x.DeliveryDirections!).ThenInclude(x => x.City).Where(x => customers.Any(y => y == x.Id)).AsNoTracking();
        }
    }
}
