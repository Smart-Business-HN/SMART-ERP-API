using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.QuotationSnapshotSpecification
{
    public class GetQuotationSnapshotByIdSpecification : Specification<QuotationSnapshot>
    {
        public GetQuotationSnapshotByIdSpecification(int snapshotId)
        {
            Query.Where(x => x.Id == snapshotId).AsNoTracking();
        }
    }
}
