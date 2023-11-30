using Ardalis.Specification;
using SMART.ERP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.ERP.Application.Specifications.InvoiceSpecification
{
    public class FilterInvoiceInYearByBranchSpecification : Specification<Invoice>
    {
        public FilterInvoiceInYearByBranchSpecification(int year, int branchId) {
            Query.Include(x => x.ProductsSold!)
               .ThenInclude(x => x.Product).Where(x =>  x.CreationDate.Year == year && x.User!.BranchOfficeId == branchId).AsNoTracking();
        }
    }
}
