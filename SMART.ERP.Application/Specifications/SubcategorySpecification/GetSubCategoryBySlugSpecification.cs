using Ardalis.Specification;
using SMART.ERP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.ERP.Application.Specifications.SubcategorySpecification
{
    public class GetSubCategoryBySlugSpecification : Specification<Subcategory>
    {
        public GetSubCategoryBySlugSpecification(string subCategorySlug) {
            Query.Where(x=>x.Slug == subCategorySlug);
        }
    }
}
