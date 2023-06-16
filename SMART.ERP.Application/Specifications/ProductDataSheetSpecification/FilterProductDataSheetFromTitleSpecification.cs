using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.ProductDataSheetSpecification
{
    public class FilterProductDataSheetFromTitleSpecification : Specification<ProductDataSheet>
    {
        public FilterProductDataSheetFromTitleSpecification(string title)
        {
            Query.Where(a => a.Title == title).AsNoTracking();
        }
    }
}
