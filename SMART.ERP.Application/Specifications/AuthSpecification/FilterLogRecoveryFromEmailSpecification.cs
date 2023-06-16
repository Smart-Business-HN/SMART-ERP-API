using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.AuthSpecification
{
    public class FilterLogRecoveryFromEmailSpecification : Specification<LogRecovery>
    {
        public FilterLogRecoveryFromEmailSpecification(string email)
        {
            Query.Where(x => x.Email == email);
        }
    }
}
