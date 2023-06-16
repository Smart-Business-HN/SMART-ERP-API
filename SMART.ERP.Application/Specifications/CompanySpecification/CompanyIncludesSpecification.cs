using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.CompanySpecification
{
    public class CompanyIncludesSpecification : Specification<Company>
    {
        public CompanyIncludesSpecification()
        {
            Query.Include(x => x.Banners).Include(x => x.BranchOffices).Include(x => x.Opinions).AsNoTracking();
        }
    }
}
