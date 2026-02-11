using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.LogEcommerceUserSpecification;

public sealed class FilterLogEcommerceUserSpecification : Specification<LogEcommerceUser>
{
    public FilterLogEcommerceUserSpecification(Guid ecommerceUserId, int? actionType, int pageNumber, int pageSize)
    {
        Query.Where(x => x.EcommerceUserId == ecommerceUserId);

        if (actionType.HasValue)
        {
            Query.Where(x => x.ActionType == actionType.Value);
        }

        Query.OrderByDescending(x => x.CreationDate)
            .Skip(pageNumber * pageSize)
            .Take(pageSize)
            .AsNoTracking();
    }
}
