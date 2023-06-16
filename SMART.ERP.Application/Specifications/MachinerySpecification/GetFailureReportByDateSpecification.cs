using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.MachinerySpecification
{
    public class GetFailureReportByDateSpecification : Specification<MachineryFailureReport>
    {
        public GetFailureReportByDateSpecification(DateTime initDate, DateTime endDate)
        {
            Query.Include(i => i.Status).Include(i => i.MachineryFailure)
                .Where(a => a.CreationDate.Date >= initDate && a.CreationDate.Date <= endDate).AsNoTracking();
        }
    }
}
