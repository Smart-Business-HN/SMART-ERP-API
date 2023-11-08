using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.HeroSliderSpecification
{
    public class IncludeHeroSliderSpecification : Specification<Category>
    {
        public IncludeHeroSliderSpecification(int? id)
        {
            if (id == 0)
            {
                Query.Include(x => x.HeroSliders!).ThenInclude(x => x.Product!).ThenInclude(x => x.Status)
                    .Include(x => x.HeroSliders!).ThenInclude(x => x.Product!).ThenInclude(x => x.ProductImages)
                    .Include(x => x.HeroSliders!).ThenInclude(x => x.Product!).ThenInclude(x => x.SubCategory).ThenInclude(x => x.Category)
                .Include(x => x.HeroSliders!.OrderBy(x => x.Position)).ThenInclude(x => x.Product!).ThenInclude(x => x.ProductDataSheets!).ThenInclude(x => x.DataSheet)
                .Where(x => x.Position != 0).OrderBy(x => x.Position).AsNoTracking();
            }
            else
            {
                Query.Include(x => x.HeroSliders!).ThenInclude(x => x.Product!).ThenInclude(x => x.Status)
                     .Include(x => x.HeroSliders!).ThenInclude(x => x.Product!).ThenInclude(x => x.ProductImages)
                     .Include(x => x.HeroSliders!).ThenInclude(x => x.Product!).ThenInclude(x => x.SubCategory).ThenInclude(x=>x.Category)
                .Include(x => x.HeroSliders!.OrderBy(x => x.Position)).ThenInclude(x => x.Product!).ThenInclude(x => x.ProductDataSheets!).ThenInclude(x => x.DataSheet)
                .Where(x => x.Id == id).AsNoTracking();
            }

        }
    }
}
