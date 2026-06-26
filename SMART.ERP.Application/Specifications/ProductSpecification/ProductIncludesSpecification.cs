using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.ProductSpecification
{
    public class ProductIncludesSpecification : Specification<Product>
    {
        public ProductIncludesSpecification(int? id, string? slug)
        {
            if (id != null)
            {
                var _cutOffDate = DateTime.Now.AddMonths(-12);
                Query.Include(x => x.Brand).Include(x => x.Status).Include(x => x.SubCategory)
                    .Include(x => x.ProductSubcategories!).ThenInclude(ps => ps.Subcategory).ThenInclude(s => s!.Category)
                    .Include(x => x.ProductDataSheets!).ThenInclude(x => x.DataSheet).Include(x => x.ProductFeatures)
                    .Include(x => x.ProductImages).Include(x => x.UnitOfMeasurement)
                    .Include(x => x.ProductPurchasePriceLogs!.Where(a => a.PurchaseDate >= _cutOffDate))
                    .Include(x => x.Components!.Where(c => c.IsActive)).ThenInclude(c => c.Product)
                    .Where(x => x.Id == id).AsNoTracking();
            }
            else if (id == null && slug != null)
            {
                Query.Include(x => x.Brand).Include(x => x.Status).Include(x => x.SubCategory).ThenInclude(x => x!.Category).Include(x => x.Tax)
                    .Include(x => x.ProductSubcategories!).ThenInclude(ps => ps.Subcategory).ThenInclude(s => s!.Category)
                    .Include(x => x.ProductDataSheets!).ThenInclude(x => x.DataSheet).Include(x => x.ProductFeatures)
                    .Include(x => x.ProductImages).Where(x => x.Slug == slug).AsNoTracking();
            }
            else
            {
                Query.Include(x => x.Brand).Include(x => x.Status).Include(x => x.SubCategory)
                    .Include(x => x.ProductDataSheets!).ThenInclude(x => x.DataSheet).Include(x => x.ProductFeatures)
                    .Include(x => x.ProductImages).Include(x => x.UnitOfMeasurement).AsNoTracking();
            }
        }
    }
}
