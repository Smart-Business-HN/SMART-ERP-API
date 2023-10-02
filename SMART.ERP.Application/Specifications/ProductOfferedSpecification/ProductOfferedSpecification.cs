using Ardalis.Specification;
using SMART.ERP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.ERP.Application.Specifications.ProductOfferedSpecification
{
    public class ProductOfferedSpecification : Specification<ProductOffered>
    {
        public ProductOfferedSpecification(int quotationId) 
        {
            Query.Include(x => x.Tax).Include(x=>x.Product).Where(x => x.QuotationId == quotationId);
        }
    }
}
