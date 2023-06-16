using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.ProductFeatureSpecification
{
    public class FilterProductFeatureFromTitleSpecification : Specification<ProductFeature>
    {
        public FilterProductFeatureFromTitleSpecification(string title, int? productId)
        {
            if (title != null)
                Query.Where(a => a.Title == title && a.ProductId == productId).AsNoTracking();
            else
                Query.Where(a => a.Title == title).AsNoTracking();
        }
    }
}
