using Ardalis.Specification;
using SMART.ERP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.ERP.Application.Specifications.InvoiceSpecification
{
    public class FilterInvoicesInYearByUserSpecification : Specification<Invoice>
    {
        public FilterInvoicesInYearByUserSpecification(int year, Guid? id) {
            Query.Include(x => x.ProductsSold!).ThenInclude(x => x.Product)
                    .ThenInclude(x => x!.SubCategory)
                        .Where(x=> x.CreationDate.Year == year).AsNoTracking();
            if (id != null)
            {
                Query.Where(x => x.UserId == id);
            }
        }
    }
}
