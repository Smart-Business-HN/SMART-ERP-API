using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.LogEcommerceUserSpecification;

public sealed class CountLogEcommerceUserSpecification : Specification<LogEcommerceUser>
{
    public CountLogEcommerceUserSpecification(Guid ecommerceUserId, int? actionType)
    {
        Query.Where(x => x.EcommerceUserId == ecommerceUserId);

        if (actionType.HasValue)
        {
            Query.Where(x => x.ActionType == actionType.Value);
        }
    }
}
