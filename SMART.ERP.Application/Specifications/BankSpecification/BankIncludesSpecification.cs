using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.BankSpecification
{
    public class BankIncludesSpecification : Specification<Bank>
    {
        public BankIncludesSpecification(int id) {
            Query.Include(x => x.InternalBankAccounts).Where(x => x.Id == id);
        }
    }
}
