using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.IncomeAccountSpecification
{
    public class FilterIncomeAccountFromNameSpecification : Specification<IncomeAccount>
    {
        public FilterIncomeAccountFromNameSpecification(string name)
        {
            Query.Where(x => x.Name == name);
        }
    }
}
