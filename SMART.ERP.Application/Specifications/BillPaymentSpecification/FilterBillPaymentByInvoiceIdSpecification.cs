using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.BillPaymentSpecification
{
    public class FilterBillPaymentByInvoiceIdSpecification : Specification<BillPayment>
    {
        public FilterBillPaymentByInvoiceIdSpecification(int InvoiceId) {
            Query.Include(x=>x.Invoice).Include(x=>x.InternalBankAccount).ThenInclude(x=>x!.Bank).Include(x=>x.TypeOfPaymentMethod).Where(x=>x.InvoiceId == InvoiceId);
        }
    }
}
