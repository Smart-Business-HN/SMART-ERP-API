using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.AuthSpecification
{
    public class FilterLogRecoveryFromDateToday : Specification<LogRecovery>
    {
        public FilterLogRecoveryFromDateToday(string email, DateTime date)
        {
            Query.Where(x => x.ExpirationDate.DayOfYear == date.DayOfYear && x.Email == email).AsNoTracking();
        }
    }
}
