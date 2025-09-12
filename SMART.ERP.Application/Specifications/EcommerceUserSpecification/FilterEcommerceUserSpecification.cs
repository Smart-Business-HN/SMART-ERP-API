using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.EcommerceUserSpecification;

public sealed class FilterEcommerceUserSpecification : Specification<EcommerceUser>
{
    public FilterEcommerceUserSpecification(string filter, Guid? id)
    {
        Query.Include(x=>x.CustomerType);
        if (id.HasValue)
        {
            Query.Where(x=>x.Id==id.Value);
        }
        else
        {
            Query.Where(x => x.Email.ToLower() == filter.ToLower() || x.UserName.ToLower() == filter.ToLower());
        }
    }
}