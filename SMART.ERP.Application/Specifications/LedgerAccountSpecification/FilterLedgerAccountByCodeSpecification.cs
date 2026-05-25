using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.LedgerAccountSpecification
{
    public class FilterLedgerAccountByCodeSpecification : Specification<LedgerAccount>
    {
        public FilterLedgerAccountByCodeSpecification(string code)
        {
            Query.Where(x => x.Code == code).AsNoTracking();
        }
    }
}
