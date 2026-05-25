using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.LedgerAccountSpecification
{
    /// <summary>Todas las cuentas ordenadas por código (para armar el árbol del catálogo).</summary>
    public class AllLedgerAccountsOrderedSpecification : Specification<LedgerAccount>
    {
        public AllLedgerAccountsOrderedSpecification(bool onlyPostable = false)
        {
            Query.OrderBy(x => x.Code).AsNoTracking();
            if (onlyPostable)
            {
                Query.Where(x => x.IsPostable && x.IsActive);
            }
        }
    }
}
