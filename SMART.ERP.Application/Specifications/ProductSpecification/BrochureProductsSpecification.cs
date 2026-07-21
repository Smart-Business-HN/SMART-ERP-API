using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.ProductSpecification
{
    /// <summary>
    /// Productos que entran a un brochure: activos, publicados en ecommerce y con
    /// existencia (física o virtual), filtrados por marca/categoría/subcategoría.
    ///
    /// Semántica de facetas: <b>AND entre facetas, OR dentro de cada faceta</b>.
    /// Elegir "Hikvision + Ubiquiti" y "Cámaras" devuelve las cámaras de esas dos
    /// marcas, no todo lo de las marcas más todo lo de la categoría.
    /// </summary>
    public sealed class BrochureProductsSpecification : Specification<Product>
    {
        public BrochureProductsSpecification(
            IReadOnlyCollection<int>? brandIds,
            IReadOnlyCollection<int>? categoryIds,
            IReadOnlyCollection<int>? subcategoryIds)
        {
            Query.Include(x => x.Brand)
                 .Include(x => x.SubCategory)
                     .ThenInclude(s => s!.Category)
                 .Include(x => x.Tax)
                 .Include(x => x.ProductImages)
                 .AsNoTracking();

            // No se hace Include de InventoryDistributions: el predicado de stock se
            // traduce a un EXISTS y no necesita materializar la colección. Incluirla
            // multiplicaría las filas sin aportar nada al documento.

            BrochureProductCriteria.Apply(Query, brandIds, categoryIds, subcategoryIds);

            // El brochure se arma agrupado por categoría, así que ese es el orden primario.
            Query.OrderBy(x => x.SubCategory!.Category!.Name)
                 .ThenBy(x => x.Brand!.Name)
                 .ThenBy(x => x.Name);
        }
    }

    /// <summary>
    /// Misma criteria, sin Include ni OrderBy. Sirve para contar antes de
    /// materializar nada (guarda de escala y vista previa).
    /// </summary>
    public sealed class BrochureProductsCountSpecification : Specification<Product>
    {
        public BrochureProductsCountSpecification(
            IReadOnlyCollection<int>? brandIds,
            IReadOnlyCollection<int>? categoryIds,
            IReadOnlyCollection<int>? subcategoryIds)
        {
            Query.AsNoTracking();
            BrochureProductCriteria.Apply(Query, brandIds, categoryIds, subcategoryIds);
        }
    }

    /// <summary>
    /// Criteria compartida por las dos specs de arriba, para que contar y listar no
    /// puedan divergir. Mismo idiom que <see cref="ProductSearchPredicate"/>.
    /// </summary>
    internal static class BrochureProductCriteria
    {
        public static void Apply(
            ISpecificationBuilder<Product> query,
            IReadOnlyCollection<int>? brandIds,
            IReadOnlyCollection<int>? categoryIds,
            IReadOnlyCollection<int>? subcategoryIds)
        {
            query.Where(x => x.ShowInEcommerce && x.IsActive);

            // Stock físico O virtual — idéntico a OptimizedProductSearchSpecification,
            // para que el brochure no ofrezca algo que la tienda considera agotado.
            query.Where(x => x.CurrentStock > 0 ||
                x.InventoryDistributions!.Any(d => d.Warehouse!.IsVirtual && d.Quantity > 0));

            if (brandIds is { Count: > 0 })
            {
                query.Where(x => brandIds.Contains(x.BrandId));
            }

            // Categoría y subcategoría van por la tabla puente ProductSubcategoryLink.
            // Product.SubCategoryId es solo la canónica (URLs, breadcrumbs, SEO) y
            // filtrar por ella dejaría fuera productos multi-categoría.
            if (categoryIds is { Count: > 0 })
            {
                query.Where(x => x.ProductSubcategories!
                    .Any(ps => categoryIds.Contains(ps.Subcategory!.CategoryId)));
            }

            if (subcategoryIds is { Count: > 0 })
            {
                query.Where(x => x.ProductSubcategories!
                    .Any(ps => subcategoryIds.Contains(ps.SubcategoryId)));
            }

            // El borrado lógico lo cubre el HasQueryFilter global (p => !p.IsDeleted).
        }
    }
}
