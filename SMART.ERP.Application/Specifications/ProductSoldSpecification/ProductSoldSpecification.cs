using Ardalis.Specification;
using SMART.ERP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.ERP.Application.Specifications.ProductSoldSpecification
{
    public class ProductSoldSpecification : Specification<ProductSold>
    {
        public ProductSoldSpecification(int invoiceId) 
        {
            Query.Include(x => x.Product).Include(x=>x.Tax).Where(x=>x.InvoiceId == invoiceId);
        }
    }
}
