using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.ProspectSpecification
{
    public class FilterConvertedProspectSpecification : Specification<Prospect>
    {
        public FilterConvertedProspectSpecification(DateTime? date)
        {
            Query.Where(x => x.ProspectStep!.Name == "Convertido");
            if (date != null)
            {
                Query.Where(x => x.ModificationDate.HasValue && x.ModificationDate.Value.Month == date.Value.Month &&
                x.ModificationDate.Value.Year == date.Value.Year);
            }
        }
    }
}
