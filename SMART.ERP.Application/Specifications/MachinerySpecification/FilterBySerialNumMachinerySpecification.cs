using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.MachinerySpecification
{
    public class FilterBySerialNumMachinerySpecification : Specification<Machinery>
    {
        public FilterBySerialNumMachinerySpecification(string serie, int? id)
        {
            if (id > 0 && id != null && !string.IsNullOrWhiteSpace(serie))
                Query.Where(a => a.Id != id && a.SerialNum == serie);
            else
                Query.Where(a => a.SerialNum == serie);
        }
    }
}
