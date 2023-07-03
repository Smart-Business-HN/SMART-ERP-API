using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.ClientSpecification
{
    public class FilterClientFromEmailSpecification : Specification<Customer>
    {
        public FilterClientFromEmailSpecification(string email)
        {
            Query.Where(x => x.Email == email).AsNoTracking();
        }
    }
}
