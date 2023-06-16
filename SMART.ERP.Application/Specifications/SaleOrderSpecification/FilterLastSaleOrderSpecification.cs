using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.SaleOrderSpecification
{
    public class FilterLastSaleOrderSpecification : Specification<SaleOrder>
    {
        public FilterLastSaleOrderSpecification()
        {
            Query.OrderByDescending(x => x.Id).Take(1).AsNoTracking();
        }
    }
}
