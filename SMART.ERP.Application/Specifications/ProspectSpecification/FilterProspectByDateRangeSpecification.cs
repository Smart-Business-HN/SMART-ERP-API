using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.ProspectSpecification
{
    public class FilterProspectByDateRangeSpecification : Specification<Prospect>
    {
        public FilterProspectByDateRangeSpecification(DateTime? StartDate, DateTime? EndDate)
        {
            Query.Include(x => x.ProspectStep).AsNoTracking();
            if (StartDate != null)
            {
                Query.Where(x => x.CreationDate.Date >= StartDate);
            }
            if (EndDate != null)
            {
                Query.Where(x => x.CreationDate.Date <= EndDate);
            }
        }
    }
}
