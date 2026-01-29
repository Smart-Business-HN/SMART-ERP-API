using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.BankSpecification
{
    public class FilterBankFromNameSpecification : Specification<Bank>
    {
        public FilterBankFromNameSpecification(string name) {
        Query.Where(x=>x.Name == name);
        }
    }
}
