using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.QuotationSpecification
{
    public class FilterQuotationByAccessTokenSpecification : Specification<Quotation>
    {
        public FilterQuotationByAccessTokenSpecification(Guid accessToken)
        {
            Query.Include(x => x.Status)
                 .Include(x => x.Customer).ThenInclude(x => x!.DeliveryDirections)
                 .Include(x => x.User)
                 .Include(x => x.ProductsOffered)!.ThenInclude(x => x.Tax)
                 .Include(x => x.ProductsOffered)!.ThenInclude(x => x.Product!).ThenInclude(p => p.Components!).ThenInclude(c => c.Product!).ThenInclude(cp => cp!.UnitOfMeasurement)
                 .Include(x => x.Comments)!.ThenInclude(x => x.User)
                 .Include(x => x.ItemObservations)
                 .Where(x => x.AccessToken == accessToken)
                 .AsNoTrackingWithIdentityResolution();
        }
    }
}
