using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP .Application.Specifications.BannerSpecification
{
    public class FilterBannerFromNameSpecification : Specification<Banner>
    {
        public FilterBannerFromNameSpecification(string fileName)
        {
            Query.Where(a => a.FileName == fileName).AsNoTracking();
        }
    }
}
