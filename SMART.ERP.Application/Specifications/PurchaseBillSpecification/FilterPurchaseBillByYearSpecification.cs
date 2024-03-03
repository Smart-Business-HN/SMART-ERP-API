using Ardalis.Specification;
using SMART.ERP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.ERP.Application.Specifications.PurchaseBillSpecification
{
    public class FilterPurchaseBillByYearSpecification : Specification<PurchaseBill>
    {
        public FilterPurchaseBillByYearSpecification(DateTime date)
        {
            Query.Include(x=>x.ExpenseAccount).Where(x => x.CreationDate.Year == date.Year);
        }
    }
}
