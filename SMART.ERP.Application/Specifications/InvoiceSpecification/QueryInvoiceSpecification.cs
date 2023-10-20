using Ardalis.Specification;
using SMART.ERP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.ERP.Application.Specifications.InvoiceSpecification
{
    public class QueryInvoiceSpecification : Specification<Invoice>
    {
        public QueryInvoiceSpecification(string? parameter, int pageNumber,
           int pageSize, string? order, string? column)
        {
            Query.Include(x => x.ProductsSold!).ThenInclude(x => x.Product).ThenInclude(x => x!.Brand)
              .Include(x => x.ProductsSold!).ThenInclude(x => x.Tax)
              .Include(x => x.BranchOffice!)
              .Include(x => x.User!)
              .Include(x => x.Cai)
              .Include(x => x.Status)
              .Include(x => x.Customer).Include(x => x.User).Skip((pageNumber) * pageSize).Take(pageSize).OrderByDescending(x => x.Id).AsNoTracking();

            if (!string.IsNullOrEmpty(parameter))
            {
                Query.Where(x => x.InvoiceNumber.Contains(parameter) || x.Customer.FullName.Contains(parameter));
            }

            if (!string.IsNullOrEmpty(order) && !string.IsNullOrEmpty(column))
            {
                if (order == "desc")
                {
                    Query.OrderBy(x => column == "InvoiceNumber" ? x.InvoiceNumber : null);
                }
                else
                {
                    Query.OrderBy(x => column == "InvoiceNumber" ? x.InvoiceNumber : null);
                }
            }
        }
    }
}
