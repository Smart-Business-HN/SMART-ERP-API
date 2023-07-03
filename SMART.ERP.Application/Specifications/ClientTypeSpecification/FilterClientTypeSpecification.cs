using Ardalis.Specification;
using SMART.ERP.Domain.Entities;


namespace SMART.ERP.Application.Specifications.ClientTypeSpecification
{
    public class FilterClientTypeSpecification : Specification<CustomerType>
    {
        public FilterClientTypeSpecification(string name)
        {
            Query.Where(x => x.Name == name).AsNoTracking();
        }
    }
}
