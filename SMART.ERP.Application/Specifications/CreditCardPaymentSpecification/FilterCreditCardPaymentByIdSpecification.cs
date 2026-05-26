using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.CreditCardPaymentSpecification
{
    public class FilterCreditCardPaymentByIdSpecification : Specification<CreditCardPayment>
    {
        public FilterCreditCardPaymentByIdSpecification(int id)
        {
            Query.Include(x => x.CreditCardInternalBankAccount)
                 .Include(x => x.SourceInternalBankAccount)
                 .Where(x => x.Id == id)
                 .AsNoTracking();
        }
    }
}
