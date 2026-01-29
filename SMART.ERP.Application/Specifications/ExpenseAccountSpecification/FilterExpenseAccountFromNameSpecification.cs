using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.ExpenseAccountSpecification
{
    public class FilterExpenseAccountFromNameSpecification : Specification<ExpenseAccount>
    {
        public FilterExpenseAccountFromNameSpecification(string name)
        {
            Query.Where(x => x.Name == name);
        }
    }
}
