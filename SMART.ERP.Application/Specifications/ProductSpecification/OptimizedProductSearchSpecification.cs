using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.ProductSpecification
{
    public sealed class OptimizedProductSearchSpecification : Specification<Product>
    {
        public OptimizedProductSearchSpecification(
            string? searchTerm,
            int pageNumber,
            int pageSize,
            string? order,
            string? column,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            int? brandId = null,
            int? categoryId = null,
            int? subCategoryId = null,
            bool? inStock = null,
            string? sortBy = null)
        {
            // Includes optimizados - solo los necesarios para ecommerce
            Query.Include(x => x.SubCategory)
                .ThenInclude(x => x!.Category)
                .Include(x => x.Brand)
                .Include(x => x.Status)
                .Include(x => x.Tax)
                .Include(x => x.ProductImages)
                .Where(x => x.ShowInEcommerce && x.IsActive)
                .AsNoTracking();

            // Búsqueda multi-término con relevancia, insensible a mayúsculas y acentos.
            ProductSearchPredicate.Apply(Query, searchTerm, useEcommerceFields: true);

            // Filtros adicionales
            if (minPrice.HasValue)
            {
                Query.Where(x => x.RecomendedSalePrice >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                Query.Where(x => x.RecomendedSalePrice <= maxPrice.Value);
            }

            if (brandId.HasValue)
            {
                Query.Where(x => x.BrandId == brandId.Value);
            }

            if (categoryId.HasValue)
            {
                Query.Where(x => x.ProductSubcategories!.Any(ps => ps.Subcategory!.CategoryId == categoryId.Value));
            }

            if (subCategoryId.HasValue)
            {
                Query.Where(x => x.ProductSubcategories!.Any(ps => ps.SubcategoryId == subCategoryId.Value));
            }

            if (inStock.HasValue && inStock.Value)
            {
                // Include con InventoryDistributions para verificar stock virtual
                Query.Include(x => x.InventoryDistributions!)
                    .ThenInclude(d => d.Warehouse);

                // Mostrar productos con stock físico O virtual
                Query.Where(x => x.CurrentStock > 0 ||
                    x.InventoryDistributions!.Any(d =>
                        d.Warehouse!.IsVirtual && d.Quantity > 0));
            }

            // Ordenamiento: relevancia por defecto cuando hay búsqueda; respeta sortBy/column explícitos.
            ProductSearchPredicate.ApplyOrdering(
                Query, searchTerm, sortBy, useEcommerceFields: true, legacyOrder: order, legacyColumn: column);

            // Paginación
            Query.Skip(pageNumber * pageSize).Take(pageSize);
        }
    }
}

