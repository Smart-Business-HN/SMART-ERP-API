using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.InternalBankAccountSpecification
{
    public class FilterInternalBankAccountFromNameSpecification : Specification<InternalBankAccount>
    {
        public FilterInternalBankAccountFromNameSpecification(string name)
        {
            Query.Where(x=>x.Name == name);
        }
    }
}
