using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.MajorExpenseAccountSpecification
{
    public class FilterMajorExpenseAccountFromNameSpecification : Specification<MajorExpenseAccount>
    {
        public FilterMajorExpenseAccountFromNameSpecification(string name)
        {
            Query.Where(x => x.Name == name);
        }
    }
}
