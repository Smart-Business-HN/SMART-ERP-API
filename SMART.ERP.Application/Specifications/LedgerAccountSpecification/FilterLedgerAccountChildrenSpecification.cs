using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.LedgerAccountSpecification
{
    public class FilterLedgerAccountChildrenSpecification : Specification<LedgerAccount>
    {
        public FilterLedgerAccountChildrenSpecification(int parentId)
        {
            Query.Where(x => x.ParentId == parentId).AsNoTracking();
        }
    }
}
