using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.PaymentMethodSpecification;

public sealed class FilterPaymentMethodSpecification : Specification<PaymentMethod>
{
    public FilterPaymentMethodSpecification(string alias, Guid ecommerceUserId, int? id)
    {
        if (id != null)
            Query.Where(x => x.Alias == alias && x.EcommerceUserId == ecommerceUserId && x.Id != id).AsNoTracking();
        else
            Query.Where(x => x.Alias == alias && x.EcommerceUserId == ecommerceUserId).AsNoTracking();
    }
}
