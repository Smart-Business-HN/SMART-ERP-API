using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.TypeStatusSpecification
{
    public class TypeStatusIncludesSpecification : Specification<TypeStatus>
    {
        public TypeStatusIncludesSpecification(int? id)
        {
            if (id != null || id > 0)
            {
                Query.Include(x => x.Status).Where(x => x.Id == id).AsNoTracking();
            }
            else
            {
                Query.Include(x => x.Status).AsNoTracking();
            }
        }
    }
}
