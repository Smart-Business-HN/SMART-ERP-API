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
            bool? inStock = null)
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

            // Búsqueda de texto optimizada
            if (!string.IsNullOrEmpty(searchTerm))
            {
                var searchTermLower = searchTerm.ToLower();
                
                Query.Where(x => 
                    // Búsqueda exacta en nombre (mayor relevancia)
                    x.Name.ToLower().Contains(searchTermLower) ||
                    // Búsqueda exacta en código
                    x.Code.ToLower().Contains(searchTermLower) ||
                    // Búsqueda en descripción del ecommerce
                    (x.EcommerceDescription != null && x.EcommerceDescription.ToLower().Contains(searchTermLower)) ||
                    // Búsqueda en descripción general
                    (x.Description != null && x.Description.ToLower().Contains(searchTermLower)) ||
                    // Búsqueda en nombre de subcategoría
                    x.SubCategory!.Name.ToLower().Contains(searchTermLower) ||
                    // Búsqueda en nombre de categoría
                    x.SubCategory!.Category!.Name.ToLower().Contains(searchTermLower) ||
                    // Búsqueda en marca
                    x.Brand!.Name.ToLower().Contains(searchTermLower)
                );
            }

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
                Query.Where(x => x.SubCategory!.CategoryId == categoryId.Value);
            }

            if (subCategoryId.HasValue)
            {
                Query.Where(x => x.SubCategoryId == subCategoryId.Value);
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

            // Ordenamiento optimizado
            if (!string.IsNullOrEmpty(order) && !string.IsNullOrEmpty(column))
            {
                if (order.ToLower() == "desc")
                {
                    Query.OrderByDescending(GetOrderExpression(column));
                }
                else
                {
                    Query.OrderBy(GetOrderExpression(column));
                }
            }
            else
            {
                // Ordenamiento por defecto: relevancia y luego por nombre
                Query.OrderBy(x => x.Name);
            }

            // Paginación
            Query.Skip(pageNumber * pageSize).Take(pageSize);
        }

        private static System.Linq.Expressions.Expression<System.Func<Product, object?>> GetOrderExpression(string column)
        {
            return column.ToLower() switch
            {
                "name" => x => x.Name,
                "code" => x => x.Code,
                "price" => x => x.RecomendedSalePrice,
                "brand" => x => x.Brand!.Name,
                "category" => x => x.SubCategory!.Category!.Name,
                "subcategory" => x => x.SubCategory!.Name,
                "stock" => x => x.CurrentStock,
                "createdate" => x => x.CreationDate,
                _ => x => x.Name
            };
        }
    }
}

