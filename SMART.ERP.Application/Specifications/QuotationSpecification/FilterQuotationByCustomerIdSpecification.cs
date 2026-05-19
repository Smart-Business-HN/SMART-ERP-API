using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.QuotationSpecification
{
    public class FilterQuotationByCustomerIdSpecification : Specification<Quotation>
    {
        public FilterQuotationByCustomerIdSpecification(Guid customerId, string? parameter, int pageNumber, int pageSize)
        {
            Query
                .Include(x => x.Status)
                .Where(x => x.CustomerId == customerId)
                .OrderByDescending(x => x.CreationDate)
                .AsNoTracking();

            if (!string.IsNullOrEmpty(parameter))
            {
                Query.Where(x => x.QuotationCode!.Contains(parameter));
            }

            Query.Skip(pageNumber * pageSize).Take(pageSize);
        }
    }

    public class CountQuotationByCustomerIdSpecification : Specification<Quotation>
    {
        public CountQuotationByCustomerIdSpecification(Guid customerId, string? parameter)
        {
            Query
                .Where(x => x.CustomerId == customerId)
                .AsNoTracking();

            if (!string.IsNullOrEmpty(parameter))
            {
                Query.Where(x => x.QuotationCode!.Contains(parameter));
            }
        }
    }
}
