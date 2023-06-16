using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.MachinerySpecification
{
    public class FilterByNameMachinerySpecification : Specification<Machinery>
    {
        public FilterByNameMachinerySpecification(string name, int? id)
        {
            if (id > 0 && id != null && !string.IsNullOrWhiteSpace(name))
                Query.Where(a => a.Id != id && a.DeviceName == name);
            else
                Query.Where(a => a.DeviceName == name);
        }
    }
}
