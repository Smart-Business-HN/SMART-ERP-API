using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.PaymentMethodSpecification;

public sealed class FilterAndPaginationPaymentMethodSpecification : Specification<PaymentMethod>
{
    public FilterAndPaginationPaymentMethodSpecification(Guid ecommerceUserId, string? parameter, int pageNumber, int pageSize, string? order, string? column)
    {
        Query.Where(x => x.EcommerceUserId == ecommerceUserId);

        Query.Skip(pageNumber * pageSize).Take(pageSize).AsNoTracking();

        if (!string.IsNullOrEmpty(parameter))
        {
            Query.Where(x => x.Alias.Contains(parameter) || x.CardholderName.Contains(parameter) || x.Last4Digits.Contains(parameter));
        }

        if (!string.IsNullOrEmpty(order) && !string.IsNullOrEmpty(column))
        {
            if (order == "desc")
            {
                Query.OrderByDescending(x => column == "Alias" ? x.Alias : column == "CardholderName" ? x.CardholderName : null);
            }
            else
            {
                Query.OrderBy(x => column == "Alias" ? x.Alias : column == "CardholderName" ? x.CardholderName : null);
            }
        }
    }
}
