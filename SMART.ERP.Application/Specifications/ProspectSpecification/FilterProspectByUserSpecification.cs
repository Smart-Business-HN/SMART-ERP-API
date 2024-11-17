using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.ProspectSpecification
{
    public class FilterProspectByUserSpecification : Specification<Prospect>
    {
        public FilterProspectByUserSpecification(Guid userId)
        {
            Query.Where(x => x.UserId == userId && x.ProspectStep!.Name != "Convertido" && x.ProspectStep.Name != "No Calificado");
        }
    }
}
