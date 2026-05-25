using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.LedgerAccountSpecification
{
    public class FilterLedgerAccountByIdSpecification : Specification<LedgerAccount>
    {
        public FilterLedgerAccountByIdSpecification(int id)
        {
            Query.Include(x => x.Parent).Where(x => x.Id == id);
        }
    }
}
