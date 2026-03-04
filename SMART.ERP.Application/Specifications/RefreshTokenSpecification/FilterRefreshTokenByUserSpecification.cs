using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.RefreshTokenSpecification
{
    public class FilterRefreshTokenByUserSpecification : Specification<RefreshToken>
    {
        public FilterRefreshTokenByUserSpecification(Guid userId)
        {
            Query.Where(x => x.UserId == userId && !x.IsRevoked);
        }
    }
}
