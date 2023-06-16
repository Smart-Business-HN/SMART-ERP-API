using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.WinReasonSpecification
{
    public class FilterWinReasonSpecification : Specification<WinReason>
    {
        public FilterWinReasonSpecification(string filter, int? id)
        {
            if (id != null) Query.Where(x => x.Name == filter && x.Id != id).AsNoTracking();
            else Query.Where(x => x.Name == filter).AsNoTracking();
        }
    }
}
