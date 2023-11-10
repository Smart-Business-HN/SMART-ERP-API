using Ardalis.Specification;
using SMART.ERP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.ERP.Application.Specifications.CategorySpecification
{
    public class GetCategoryBySlugSpecification : Specification<Category>
    {
        public GetCategoryBySlugSpecification(string slug) { 
            Query.Where(x=>x.Slug == slug);
        }
    }
}
