using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.ReportSpecification
{
    public class TopQuoteProductSpecification : Specification<Opportunity>
    {
        public TopQuoteProductSpecification(DateTime? start, DateTime? end, int? branchOfficeId, int categoryId, int? subcategoryId)
        {
            Query.Where(x => x.QuoteProducts.Any(y => y.Product.SubCategory.CategoryId == categoryId)).AsNoTracking();
            if (start != null)
            {
                Query.Where(x => x.CreationDate.Date >= start.Value.Date);
            }
            if (end != null)
            {
                Query.Where(x => x.CreationDate.Date <= end.Value.Date);
            }
            if (branchOfficeId != null)
            {
                Query.Where(x => x.User!.BranchOfficeId == branchOfficeId);
            }
            if (subcategoryId != null)
            {
                Query.Include(x => x.QuoteProducts!.Where(x => x.IsActive && x.Product.SubCategoryId == subcategoryId && x.Product.SubCategory.CategoryId == categoryId))
                    .ThenInclude(x => x.Product).ThenInclude(x => x!.SubCategory);
            }
            else
            {
                Query.Include(x => x.QuoteProducts!.Where(x => x.IsActive && x.Product.SubCategory.CategoryId == categoryId))
                .ThenInclude(x => x.Product).ThenInclude(x => x!.SubCategory);
            }
        }
    }
}
