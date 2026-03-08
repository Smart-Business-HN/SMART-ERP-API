using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.QuotationSnapshotSpecification
{
    public class GetQuotationSnapshotsByQuotationIdSpecification : Specification<QuotationSnapshot>
    {
        public GetQuotationSnapshotsByQuotationIdSpecification(int quotationId, int pageNumber, int pageSize)
        {
            Query.Where(x => x.QuotationId == quotationId)
                 .OrderByDescending(x => x.VersionNumber)
                 .Skip((pageNumber - 1) * pageSize)
                 .Take(pageSize)
                 .AsNoTracking();
        }
    }
}
