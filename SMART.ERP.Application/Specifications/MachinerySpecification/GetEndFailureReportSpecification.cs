using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.MachinerySpecification
{
    public class GetEndFailureReportSpecification : Specification<MachineryFailureReport>
    {
        public GetEndFailureReportSpecification(int machineryId, DateTime creationDate)
        {
            Query.Where(x => x.MachineryId == machineryId && x.CreationDate.Date == creationDate.Date);
        }
    }
}
