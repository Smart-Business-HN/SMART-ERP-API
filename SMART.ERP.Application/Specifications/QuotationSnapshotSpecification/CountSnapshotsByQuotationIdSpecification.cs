using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.QuotationSnapshotSpecification
{
    public class CountSnapshotsByQuotationIdSpecification : Specification<QuotationSnapshot>
    {
        public CountSnapshotsByQuotationIdSpecification(int quotationId)
        {
            Query.Where(x => x.QuotationId == quotationId);
        }
    }
}
