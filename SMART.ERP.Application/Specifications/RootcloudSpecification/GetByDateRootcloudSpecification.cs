using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.RootcloudSpecification
{
    public class GetByDateRootcloudSpecification : Specification<MachineryRootcloudHistorical>
    {
        public GetByDateRootcloudSpecification(DateTime startDate, DateTime endDate)
        {
            Query.Where(x => x.CreationDate.Date >= startDate.AddDays(-1).Date && x.CreationDate.Date <= endDate.Date);
        }
    }
}
