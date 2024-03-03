using Ardalis.Specification;
using SMART.ERP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.ERP.Application.Specifications.InvoiceSpecification
{
    public class FilterInvoiceByYearSpecification : Specification<Invoice>
    {
        public FilterInvoiceByYearSpecification(DateTime date) {
            Query.Include(x=>x.ProductsSold).ThenInclude(x=>x.Product).ThenInclude(x=>x.Brand)
                 .Include(x=>x.ProductsSold).ThenInclude(x=>x.Product).ThenInclude(x=>x.SubCategory).ThenInclude(x=>x.Category)
                 .Where(x => x.CreationDate.Year == date.Year);
        }
    }
}
