using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.OpinionSpecification
{
    public class FilterOpinionSpecification : Specification<Opinion>
    {
        public FilterOpinionSpecification(string filter, int? id)
        {
            if (id != null) Query.Where(x => x.Customer == filter && x.Id != id).AsNoTracking();
            else Query.Where(x => x.Customer == filter).AsNoTracking();
        }
    }
}
