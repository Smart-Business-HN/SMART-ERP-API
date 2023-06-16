using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.ProspectSpecification
{
    public class FilterProspectByBranchSpecification : Specification<Prospect>
    {
        public FilterProspectByBranchSpecification(int branchOfficeId)
        {
            Query.Include(x => x.ProspectStep).Where(x => x.User!.BranchOfficeId == branchOfficeId).AsNoTracking();
        }
    }
}
