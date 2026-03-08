using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.QuotationSnapshotSpecification
{
    public class GetMaxVersionSpecification : Specification<QuotationSnapshot>
    {
        public GetMaxVersionSpecification(int quotationId)
        {
            Query.Where(x => x.QuotationId == quotationId)
                 .OrderByDescending(x => x.VersionNumber)
                 .Take(1)
                 .AsNoTracking();
        }
    }
}
