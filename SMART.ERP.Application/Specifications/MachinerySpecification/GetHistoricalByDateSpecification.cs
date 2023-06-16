using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.MachinerySpecification
{
    public class GetHistoricalByDateSpecification : Specification<Machinery>
    {
        public GetHistoricalByDateSpecification(DateTime initDate, DateTime endDate)
        {
            Query.Include(i => i.MachineyRootcloudHistoricals
            .Where(s => s.CreationDate.Date >= initDate.Date && s.CreationDate.Date <= endDate.Date)).AsNoTracking();
        }
    }
}
