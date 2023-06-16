using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.ProductDataSheetSpecification
{
    public class ProductDataSheetIncludesSpecification : Specification<ProductDataSheet>
    {
        public ProductDataSheetIncludesSpecification(int? id, int? productId)
        {
            if (id != null)
            {
                Query.Include(x => x.DataSheet).Where(x => x.Id == id);
            }
            else if(productId != null)
            {
                Query.Where(x => x.ProductId == productId);
            }
            else
            {
                Query.Include(x => x.DataSheet);
            }
        }
    }
}
