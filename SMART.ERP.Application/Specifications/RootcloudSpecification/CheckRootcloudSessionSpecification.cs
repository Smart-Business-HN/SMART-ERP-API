using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.RootcloudSpecification
{
    public class CheckRootcloudSessionSpecification : Specification<RootcloudSession>
    {
        public CheckRootcloudSessionSpecification()
        {
            Query.Where(x => x.IsActive).OrderByDescending(o => o.Id);
        }
    }
}
