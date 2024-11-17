using Ardalis.Specification;
using SMART.ERP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.ERP.Application.Specifications.BillPaymentSpecification
{
    public class FilterBillPaymentByInvoiceIdSpecification : Specification<BillPayment>
    {
        public FilterBillPaymentByInvoiceIdSpecification(int InvoiceId) {
            Query.Include(x=>x.Invoice).Include(x=>x.InternalBankAccount).ThenInclude(x=>x!.Bank).Include(x=>x.TypeOfPaymentMethod).Where(x=>x.InvoiceId == InvoiceId);
        }
    }
}
