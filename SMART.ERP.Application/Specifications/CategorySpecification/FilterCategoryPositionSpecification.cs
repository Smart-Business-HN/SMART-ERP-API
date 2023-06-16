using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.CategorySpecification
{
    public class FilterCategoryPositionSpecification : Specification<Category>
    {
        public FilterCategoryPositionSpecification(int? previous, int? current)
        {
            Query.Where(x => x.Position != 0);
            if (previous != null && current != null)
            {
                if (previous < current)
                {
                    Query.Where(x => x.Position > previous && x.Position <= current);
                }
                else
                {
                    Query.Where(x => x.Position >= current && x.Position < previous);
                }
            }
            else
            {
                Query.Where(x => x.Position >= 1);
            }

        }
    }
}
