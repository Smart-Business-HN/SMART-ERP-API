using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.RefreshTokenSpecification
{
    public class FilterRefreshTokenByHashSpecification : Specification<RefreshToken>
    {
        public FilterRefreshTokenByHashSpecification(string tokenHash)
        {
            Query.Where(x => x.TokenHash == tokenHash && !x.IsRevoked)
                 .Include(x => x.User);
        }
    }
}
