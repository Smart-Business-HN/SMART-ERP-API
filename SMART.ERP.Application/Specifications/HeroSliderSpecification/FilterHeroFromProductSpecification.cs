using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.HeroSliderSpecification
{
    public class FilterHeroFromProductSpecification : Specification<HeroSlider>
    {
        public FilterHeroFromProductSpecification(int productId, int categoryId)
        {
            if (productId != 0)
            {
                Query.Where(x => x.ProductId == productId);
            }
            else
            {
                Query.Where(x => x.CategoryId == categoryId);
            }
        }
    }
}
