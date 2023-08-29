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
                Query.Include(x => x.Brand).Include(x => x.Status).Include(x => x.SubCategory)
                    .Include(x => x.Provider).Include(x => x.ProductDataSheets!).ThenInclude(x => x.DataSheet).Include(x => x.ProductFeatures)
                    .Include(x => x.ProductImages).Include(x => x.UnitOfMeasurement).Where(x => x.Id == id).AsNoTracking();
            }
            else if( id==null && slug != null)
            {
                Query.Include(x => x.Brand).Include(x => x.Status).Include(x => x.SubCategory)
                    .Include(x => x.Provider).Include(x => x.ProductDataSheets!).ThenInclude(x => x.DataSheet).Include(x => x.ProductFeatures)
                    .Include(x => x.ProductImages).Include(x => x.UnitOfMeasurement).Where(x => x.Slug == slug).AsNoTracking();
            }
            else
            {
                Query.Include(x => x.Brand).Include(x => x.Status).Include(x => x.SubCategory)
                    .Include(x => x.Provider).Include(x => x.ProductDataSheets!).ThenInclude(x => x.DataSheet).Include(x => x.ProductFeatures)
                    .Include(x => x.ProductImages).Include(x => x.UnitOfMeasurement).AsNoTracking();
            }
        }
    }
}
