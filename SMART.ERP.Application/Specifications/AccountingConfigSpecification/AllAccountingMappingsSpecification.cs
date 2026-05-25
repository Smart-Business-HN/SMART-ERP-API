using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.AccountingConfigSpecification
{
    public class AllAccountingMappingsSpecification : Specification<AccountingMapping>
    {
        public AllAccountingMappingsSpecification()
        {
            Query.Include(x => x.LedgerAccount).AsNoTracking();
        }
    }

    public class AllInternalBankAccountsWithLedgerSpecification : Specification<InternalBankAccount>
    {
        public AllInternalBankAccountsWithLedgerSpecification()
        {
            Query.Include(x => x.LedgerAccount).OrderBy(x => x.Name).AsNoTracking();
        }
    }

    public class LedgerAccountsMappedToExpenseSpecification : Specification<LedgerAccount>
    {
        public LedgerAccountsMappedToExpenseSpecification()
        {
            Query.Where(x => x.ExpenseAccountId != null).AsNoTracking();
        }
    }
}
