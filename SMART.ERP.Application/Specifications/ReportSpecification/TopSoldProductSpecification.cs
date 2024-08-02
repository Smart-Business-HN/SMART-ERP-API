using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.ReportSpecification
{
    public class TopSoldProductSpecification : Specification<Invoice>
    {
        public TopSoldProductSpecification(DateTime? start, DateTime? end, int? branchOfficeId)
        {
            Query.Include(x => x.ProductsSold!).ThenInclude(x => x.Product).ThenInclude(x => x!.SubCategory).AsNoTracking();
            if (start != null)
            {
                Query.Where(x => x.CreationDate >= start);
            }
            if (end != null)
            {
                Query.Where(x => x.CreationDate <= end);
            }
            if (branchOfficeId != null)
            {
                Query.Where(x => x.User!.BranchOfficeId == branchOfficeId);
            }
        }
    }
}
