using SMART.ERP.Domain.Entities;
using Ardalis.Specification;

namespace SMART.ERP.Application.Specifications.QuotationSpecification
{
    public class QueryQuotationSpecification : Specification<Quotation>
    {
        public QueryQuotationSpecification(string? parameter, int pageNumber,
            int pageSize, string? order, string? column) 
        {
             Query.Include(x => x.ProductsOffered!).ThenInclude(x => x.Product).ThenInclude(x => x!.Brand)
               .Include(x => x.ProductsOffered!).ThenInclude(x => x.Tax)
               .Include(x => x.BranchOffice!)
               .Include(x => x.User!)
               .Include(x=>x.Prefix)
               .Include(x=>x.Status)
               .Include(x => x.Customer).Include(x => x.User).Skip((pageNumber) * pageSize).Take(pageSize).OrderBy(x => x.QuotationCode).AsNoTracking();

            if (!string.IsNullOrEmpty(parameter))
            {
                Query.Where(x => x.QuotationCode.Contains(parameter));
            }

            if (!string.IsNullOrEmpty(order) && !string.IsNullOrEmpty(column))
            {
                if (order == "desc")
                {
                    Query.OrderByDescending(x => column == "QuotationCode" ? x.QuotationCode : null);
                }
                else
                {
                    Query.OrderBy(x => column == "QuotationCode" ? x.QuotationCode : null);
                }
            }
        }
    }
}
