using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.HeroSliderSpecification
{
    public class FilterHeroFromPositionSpecification : Specification<HeroSlider>
    {
        public FilterHeroFromPositionSpecification(int previous, int current, int categoryId)
        {
            Query.Where(x => x.CategoryId == categoryId);
            if (previous < current)
            {
                Query.Where(x => x.Position > previous && x.Position <= current);
            }
            else
            {
                Query.Where(x => x.Position >= current && x.Position < previous);
            }
        }
    }
}
