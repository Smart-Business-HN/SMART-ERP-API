using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.CaiSpecification
{
    public class FilterCaiByBranchOfficeIdSpecification : Specification<Cai>
    {
        public FilterCaiByBranchOfficeIdSpecification(int branchOfficeId)
        {
            Query.Where(x=>x.BranchOfficeId == branchOfficeId);
        }
    }
}
