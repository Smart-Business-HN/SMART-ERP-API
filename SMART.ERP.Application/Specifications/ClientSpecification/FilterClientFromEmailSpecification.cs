using Ardalis.Specification;
using SMART.MASTER.Domain.Entities;

namespace SMART.ERP.Application.Specifications.ClientSpecification
{
    public class FilterClientFromEmailSpecification : Specification<Client>
    {
        public FilterClientFromEmailSpecification(string email)
        {
            Query.Where(x => x.Email == email).AsNoTracking();
        }
    }
}
