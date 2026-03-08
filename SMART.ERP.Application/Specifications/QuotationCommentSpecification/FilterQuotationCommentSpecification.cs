using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.QuotationCommentSpecification
{
    public class FilterQuotationCommentSpecification : Specification<QuotationComment>
    {
        public FilterQuotationCommentSpecification(int quotationId, DateTime? date = null)
        {
            Query.Where(x => x.QuotationId == quotationId);

            if (date != null)
            {
                Query.Where(x => x.CreationDate.Date == date.Value.Date);
            }

            Query.OrderByDescending(x => x.CreationDate).AsNoTracking();
        }
    }
}
