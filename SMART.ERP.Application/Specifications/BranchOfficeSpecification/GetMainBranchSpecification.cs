using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.BranchOfficeSpecification
{
    public class GetMainBranchSpecification : Specification<BranchOffices>
    {
        public GetMainBranchSpecification()
        {
            Query.Where(x => x.IsMainBranchOffice).AsNoTracking();
        }
    }
}
