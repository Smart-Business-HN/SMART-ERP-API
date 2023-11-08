using Ardalis.Specification;
using SMART.ERP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.ERP.Application.Specifications.ProductSpecification
{
    public class FilterProductsBySubCategorySlugAndProductSlugSpecification : Specification<Product>
    {
        public FilterProductsBySubCategorySlugAndProductSlugSpecification(string subCategorySlug, string productSlug) {
            Query.Include(x => x.Brand)
                   .Include(x => x.ProductImages)
                   .Include(x => x.SubCategory).ThenInclude(x => x.Category)
                   .Include(x => x.Status)
                   .Where(x => x.SubCategory.Slug == subCategorySlug && x.Slug != productSlug).AsNoTracking();
        }
    }
}
