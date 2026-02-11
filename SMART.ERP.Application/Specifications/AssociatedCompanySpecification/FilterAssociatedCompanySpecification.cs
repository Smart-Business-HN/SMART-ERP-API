using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.AssociatedCompanySpecification;

public sealed class FilterAssociatedCompanySpecification : Specification<AssociatedCompany>
{
    public FilterAssociatedCompanySpecification(string name, Guid ecommerceUserId, int? id)
    {
        if (id != null)
            Query.Where(x => x.Name == name && x.EcommerceUserId == ecommerceUserId && x.Id != id).AsNoTracking();
        else
            Query.Where(x => x.Name == name && x.EcommerceUserId == ecommerceUserId).AsNoTracking();
    }
}
