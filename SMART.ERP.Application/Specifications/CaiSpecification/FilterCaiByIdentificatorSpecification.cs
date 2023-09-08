using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.CaiSpecification
{
    public class FilterCaiByIdentificatorSpecification  : Specification<Cai>
    {
        public FilterCaiByIdentificatorSpecification(string identificator)
        {
            Query.Where(x => x.Identificator == identificator).AsNoTracking();
        }
    }
}
