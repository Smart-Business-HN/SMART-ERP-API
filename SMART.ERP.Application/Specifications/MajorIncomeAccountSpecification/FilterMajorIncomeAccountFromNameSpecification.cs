using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.MajorIncomeAccountSpecification
{
    public class FilterMajorIncomeAccountFromNameSpecification : Specification<MajorIncomeAccount>
    {
        public FilterMajorIncomeAccountFromNameSpecification(string name)
        {
            Query.Where(x => x.Name == name);
        }
    }
}
