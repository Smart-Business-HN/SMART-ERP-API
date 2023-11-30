using Ardalis.Specification;
using SMART.ERP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.ERP.Application.Specifications.ProductSpecification
{
    public class ProductsForEcommerceSpecification : Specification<Product>
    {
        public ProductsForEcommerceSpecification()
        {
            Query.Where(x => x.ShowInEcommerce);
        }
    }
}
