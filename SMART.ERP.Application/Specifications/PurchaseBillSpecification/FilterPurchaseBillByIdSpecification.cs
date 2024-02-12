using Ardalis.Specification;
using SMART.ERP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.ERP.Application.Specifications.PurchaseBillSpecification
{
    public class FilterPurchaseBillByIdSpecification : Specification<PurchaseBill>
    {
        public FilterPurchaseBillByIdSpecification(int id) {
            Query.Include(x => x.Status)
                    .Include(x => x.Provider)
                    .Include(x => x.PurchaseOrderOrigin)
                    .Include(x => x.PurchaseBillPayments)!.ThenInclude(x => x.TypeOfPaymentMethod)
                    .Where(x => x.Id == id).AsNoTracking();
        }
    }
}
