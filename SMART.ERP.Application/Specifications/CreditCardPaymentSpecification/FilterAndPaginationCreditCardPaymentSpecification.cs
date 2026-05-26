using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.CreditCardPaymentSpecification
{
    public class FilterAndPaginationCreditCardPaymentSpecification : Specification<CreditCardPayment>
    {
        public FilterAndPaginationCreditCardPaymentSpecification(string? parameter, int pageNumber, int pageSize, string? order, string? column)
        {
            Query.Include(x => x.CreditCardInternalBankAccount)
                 .Include(x => x.SourceInternalBankAccount)
                 .Skip(pageNumber * pageSize)
                 .OrderByDescending(x => x.Id)
                 .Take(pageSize)
                 .AsNoTracking();

            if (!string.IsNullOrEmpty(parameter))
            {
                Query.Where(x =>
                    x.Code.Contains(parameter) ||
                    (x.Reference != null && x.Reference.Contains(parameter)) ||
                    (x.CreditCardInternalBankAccount != null && x.CreditCardInternalBankAccount.Name.Contains(parameter)) ||
                    (x.SourceInternalBankAccount != null && x.SourceInternalBankAccount.Name.Contains(parameter)));
            }

            if (!string.IsNullOrEmpty(order) && !string.IsNullOrEmpty(column))
            {
                if (order == "desc")
                    Query.OrderByDescending(x => column == "Date" ? (object)x.Date : x.Id);
                else
                    Query.OrderBy(x => column == "Date" ? (object)x.Date : x.Id);
            }
        }
    }
}
