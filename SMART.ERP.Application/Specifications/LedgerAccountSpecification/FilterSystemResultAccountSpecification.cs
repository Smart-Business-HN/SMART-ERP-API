using Ardalis.Specification;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Domain.Enums;

namespace SMART.ERP.Application.Specifications.LedgerAccountSpecification
{
    /// <summary>
    /// Cuenta de patrimonio del sistema usada para trasladar el resultado del ejercicio
    /// (Utilidad o Pérdida del Período) en el asiento de cierre.
    /// </summary>
    public class FilterSystemResultAccountSpecification : Specification<LedgerAccount>
    {
        public FilterSystemResultAccountSpecification()
        {
            Query.Where(x => x.IsSystem && x.IsPostable && x.AccountType == AccountType.Patrimonio)
                 .OrderBy(x => x.Code);
        }
    }
}
