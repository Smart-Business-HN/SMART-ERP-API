using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.ProspectStepSpecification
{
    public class FilterProspectStepSpecification : Specification<ProspectStep>
    {
        public FilterProspectStepSpecification(string name)
        {
            Query.Where(x => x.Name == name).AsNoTracking();
        }
    }
}
