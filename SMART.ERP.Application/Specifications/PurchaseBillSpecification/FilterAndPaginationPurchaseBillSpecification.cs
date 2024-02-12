using Ardalis.Specification;
using SMART.ERP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.ERP.Application.Specifications.PurchaseBillSpecification
{
    public class FilterAndPaginationPurchaseBillSpecification : Specification<PurchaseBill>
    {
        public FilterAndPaginationPurchaseBillSpecification(string? parameter, int pageNumber,
            int pageSize, string? order, string? column)
        {
            Query.Include(x => x.Provider).Include(x=>x.Status).Skip((pageNumber) * pageSize).Take(pageSize).OrderByDescending(x => x.Id).AsNoTracking();

            if (!string.IsNullOrEmpty(parameter))
            {
                Query.Where(x => x.Total.ToString().Contains(parameter) || x.Provider.Name.Contains(parameter) || x.PurchaseBillCode.Contains(parameter));
            }

            if (!string.IsNullOrEmpty(order) && !string.IsNullOrEmpty(column))
            {
                if (order == "desc")
                {
                    Query.OrderBy(x => column == "PurchaseBillCode" ? x.Id.ToString() : null);
                }
                else
                {
                    Query.OrderBy(x => column == "PurchaseBillCode" ? x.Id.ToString() : null);
                }
            }
        }
    }
}
