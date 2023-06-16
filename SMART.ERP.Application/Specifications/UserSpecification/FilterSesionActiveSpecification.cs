using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.UserSpecification
{
    public class FilterSesionActiveSpecification : Specification<LogSession>
    {
        public FilterSesionActiveSpecification(Guid userId, string? ip)
        {
            if (ip != null)
            {
                Query.Where(x => x.UserId == userId && x.IP == ip && x.IsActive);
            }
            else
            {
                Query.Where(x => x.UserId == userId && x.IsActive);
            }
        }
    }
}
