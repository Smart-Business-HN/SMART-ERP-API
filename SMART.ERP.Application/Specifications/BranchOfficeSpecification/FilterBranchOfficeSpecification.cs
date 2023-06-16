using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.BranchOfficeSpecification
{
    public class FilterBranchOfficeSpecification : Specification<BranchOffices>
    {
        public FilterBranchOfficeSpecification(string name, int? id, bool checkIfMainBranch)
        {
            if (!checkIfMainBranch)
            {
                if (id != null)
                    Query.Where(x => x.Name == name && x.Id != id).AsNoTracking();
                else
                    Query.Where(x => x.Name == name).AsNoTracking();
            }
            else
            {
                if (id != null)
                    Query.Where(x => x.IsMainBranchOffice && x.Id != id).AsNoTracking();
                else
                    Query.Where(x => x.IsMainBranchOffice).AsNoTracking();
            }
        }
    }
}
