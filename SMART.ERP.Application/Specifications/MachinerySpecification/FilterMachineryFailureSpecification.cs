using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.MachinerySpecification
{
    public class FilterMachineryFailureSpecification : Specification<MachineryFailure>
    {
        public FilterMachineryFailureSpecification(string name, int? id)
        {
            if (id != null) Query.Where(x => x.Name == name && x.Id != id).AsNoTracking();
            else Query.Where(x => x.Name == name).AsNoTracking();
        }
    }
}
