using Ardalis.Specification;
using Microsoft.Extensions.Logging;
using SMART.ERP.Application.DTOs.Brochure;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Helpers;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.BrochureImageService;
using SMART.ERP.Application.Services.PriceListResolver;
using SMART.ERP.Application.Specifications.ProductSpecification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Services.BrochureDataService
{
    public class BrochureDataService : IBrochureDataService
    {
        /// <summary>Tarjetas por página en la rejilla 2x3.</summary>
        private const int ProductsPerPage = 6;

        private readonly IReadRepositoryAsync<Product> _productRepo;
        private readonly IReadRepositoryAsync<PriceList> _priceListRepo;
        private readonly IReadRepositoryAsync<Brand> _brandRepo;
        private readonly IReadRepositoryAsync<Category> _categoryRepo;
        private readonly IPriceListService _priceListService;
        private readonly IBrochureImageService _imageService;
        private readonly ILogger<BrochureDataService> _logger;

        public BrochureDataService(
            IReadRepositoryAsync<Product> productRepo,
            IReadRepositoryAsync<PriceList> priceListRepo,
            IReadRepositoryAsync<Brand> brandRepo,
            IReadRepositoryAsync<Category> categoryRepo,
            IPriceListService priceListService,
            IBrochureImageService imageService,
            ILogger<BrochureDataService> logger)
        {
            _productRepo = productRepo;
            _priceListRepo = priceListRepo;
            _brandRepo = brandRepo;
            _categoryRepo = categoryRepo;
            _priceListService = priceListService;
            _imageService = imageService;
            _logger = logger;
        }

        public async Task<BrochurePreviewDto> GetPreviewAsync(BrochureFilterDto filter, CancellationToken ct = default)
        {
            var priceList = await RequirePriceListAsync(filter.PriceListId, ct);
            Normalize(filter);

            // La vista previa se dispara con cada cambio de filtro, así que primero se
            // cuenta. Si la selección es descomunal se responde de inmediato, sin
            // materializar miles de filas con sus imágenes y categorías.
            var matchingCount = await _productRepo.CountAsync(
                new BrochureProductsCountSpecification(filter.BrandIds, filter.CategoryIds, filter.SubCategoryIds), ct);

            if (matchingCount > BrochureLimits.MaxProducts)
            {
                return new BrochurePreviewDto
                {
                    MatchingProducts = matchingCount,
                    ProductCount = matchingCount,
                    EstimatedPages = 0,
                    ExceedsLimit = true,
                    MaxProducts = BrochureLimits.MaxProducts,
                    PriceListName = priceList.Name,
                    AppliedFilters = await BuildFilterLabelsAsync(filter, ct)
                };
            }

            var matching = await _productRepo.ListAsync(
                new BrochureProductsSpecification(filter.BrandIds, filter.CategoryIds, filter.SubCategoryIds), ct);

            var prices = await _priceListService.GetPricesFromListAsync(
                matching.Select(p => p.Id), priceList.Id, ct);

            var priced = matching.Where(p => prices.ContainsKey(p.Id) && prices[p.Id] > 0).ToList();

            return new BrochurePreviewDto
            {
                MatchingProducts = matchingCount,
                ProductCount = priced.Count,
                ProductsWithoutPrice = matchingCount - priced.Count,
                ProductsWithoutImage = priced.Count(p => FirstImageUrl(p) is null),
                EstimatedPages = EstimatePages(priced),
                ExceedsLimit = priced.Count > BrochureLimits.MaxProducts,
                MaxProducts = BrochureLimits.MaxProducts,
                PriceListName = priceList.Name,
                AppliedFilters = await BuildFilterLabelsAsync(filter, ct)
            };
        }

        public async Task<BrochureDocumentDto> BuildDocumentAsync(BrochureFilterDto filter, CancellationToken ct = default)
        {
            var priceList = await RequirePriceListAsync(filter.PriceListId, ct);
            Normalize(filter);

            // Se cuenta ANTES de materializar: una selección de "todo" cuesta un COUNT(*),
            // no traer miles de filas con sus imágenes.
            var matchingCount = await _productRepo.CountAsync(
                new BrochureProductsCountSpecification(filter.BrandIds, filter.CategoryIds, filter.SubCategoryIds), ct);

            if (matchingCount == 0)
            {
                throw new ApiException("Ningún producto cumple con los filtros seleccionados. Verifique que existan productos con existencia y publicados en el ecommerce.");
            }

            if (matchingCount > BrochureLimits.MaxProducts)
            {
                throw new ApiException(
                    $"La selección incluye {matchingCount} productos y el máximo por brochure es {BrochureLimits.MaxProducts}. Refine los filtros de marca o categoría.");
            }

            var products = await _productRepo.ListAsync(
                new BrochureProductsSpecification(filter.BrandIds, filter.CategoryIds, filter.SubCategoryIds), ct);

            // Precio ESTRICTO de la lista elegida. Nunca GetPricesAsync: aquella
            // sustituye en silencio por la lista por defecto y publicaríamos precios
            // de público en un brochure rotulado con otra lista.
            var prices = await _priceListService.GetPricesFromListAsync(
                products.Select(p => p.Id), priceList.Id, ct);

            var printable = products
                .Where(p => prices.TryGetValue(p.Id, out var price) && price > 0)
                .ToList();

            if (printable.Count == 0)
            {
                throw new ApiException(
                    $"Ninguno de los {matchingCount} productos encontrados tiene precio en la lista «{priceList.Name}». Asigne precios en esa lista o elija otra.");
            }

            // Revalidación tras excluir los sin precio: Generate no confía en Preview.
            if (printable.Count > BrochureLimits.MaxProducts)
            {
                throw new ApiException(
                    $"La selección incluye {printable.Count} productos y el máximo por brochure es {BrochureLimits.MaxProducts}. Refine los filtros de marca o categoría.");
            }

            var imageUrls = printable.Select(FirstImageUrl).Where(u => u is not null).Select(u => u!);
            var images = await _imageService.GetImagesAsync(imageUrls, ct);

            var items = printable.Select(p =>
            {
                var url = FirstImageUrl(p);
                return new BrochureProductItemDto
                {
                    ProductId = p.Id,
                    Code = p.Code,
                    Name = p.Name,
                    ShortDescription = ProductTextHelper.BuildShortDescription(p.Description, p.EcommerceDescription),
                    BrandName = p.Brand?.Name,
                    CategoryName = p.SubCategory?.Category?.Name,
                    SubCategoryName = p.SubCategory?.Name,
                    ImageUrl = url,
                    ImageBytes = url is not null && images.TryGetValue(url, out var bytes) ? bytes : null,
                    ListPrice = prices[p.Id],
                    TaxRate = p.Tax?.Rate ?? 0m
                };
            }).ToList();

            var labels = await BuildFilterLabelsAsync(filter, ct);

            _logger.LogInformation(
                "Brochure generado: {Total} productos, {WithImage} con imagen, {Placeholder} con placeholder, lista {PriceList}.",
                items.Count, items.Count(i => i.ImageBytes is not null), items.Count(i => i.ImageBytes is null), priceList.Name);

            return new BrochureDocumentDto
            {
                Title = string.IsNullOrWhiteSpace(filter.Title) ? "CATÁLOGO DE PRODUCTOS" : filter.Title!.Trim(),
                BrandsLabel = await BuildBrandLabelAsync(filter, ct),
                CategoriesLabel = await BuildCategoryLabelAsync(filter, ct),
                PriceListName = priceList.Name,
                GeneratedAt = DateTime.Now,
                Products = items,
                PlaceholderCount = items.Count(i => i.ImageBytes is null)
            };
        }

        // ── Auxiliares ────────────────────────────────────────────────────────

        private static void Normalize(BrochureFilterDto filter)
        {
            filter.BrandIds = Dedup(filter.BrandIds, BrochureLimits.MaxBrands, "marcas");
            filter.CategoryIds = Dedup(filter.CategoryIds, BrochureLimits.MaxCategories, "categorías");
            filter.SubCategoryIds = Dedup(filter.SubCategoryIds, BrochureLimits.MaxSubCategories, "subcategorías");
        }

        private static List<int> Dedup(List<int>? ids, int max, string label)
        {
            var clean = (ids ?? []).Where(id => id > 0).Distinct().ToList();
            if (clean.Count > max)
            {
                throw new ApiException($"No se pueden seleccionar más de {max} {label} a la vez.");
            }
            return clean;
        }

        private async Task<PriceList> RequirePriceListAsync(int priceListId, CancellationToken ct)
        {
            if (priceListId <= 0)
            {
                throw new ApiException("Debe seleccionar una lista de precios.");
            }

            var priceList = await _priceListRepo.FirstOrDefaultAsync(new PriceListByIdSpec(priceListId), ct)
                ?? throw new ApiException("La lista de precios seleccionada no existe.");

            if (!priceList.IsActive)
            {
                throw new ApiException($"La lista de precios «{priceList.Name}» está inactiva.");
            }

            return priceList;
        }

        /// <summary>Primera imagen del producto. No hay bandera de principal: se usa la primera.</summary>
        private static string? FirstImageUrl(Product product) =>
            product.ProductImages?
                .Select(i => i.Url)
                .FirstOrDefault(u => !string.IsNullOrWhiteSpace(u));

        /// <summary>
        /// Estima páginas contando también las bandas de categoría, que consumen alto.
        /// Un grupo nunca comparte fila con otro, así que cada categoría redondea hacia arriba.
        /// </summary>
        private static int EstimatePages(IEnumerable<Product> products)
        {
            var rows = products
                .GroupBy(p => p.SubCategory?.Category?.Name ?? "Otros")
                .Sum(g => (int)Math.Ceiling(g.Count() / 2d) + 1); // +1 fila equivalente por banda

            return Math.Max(1, (int)Math.Ceiling(rows / (double)(ProductsPerPage / 2)));
        }

        private async Task<List<string>> BuildFilterLabelsAsync(BrochureFilterDto filter, CancellationToken ct)
        {
            var labels = new List<string>();

            var brands = await BuildBrandLabelAsync(filter, ct);
            if (!string.IsNullOrWhiteSpace(brands)) labels.Add($"Marcas: {brands}");

            var categories = await BuildCategoryLabelAsync(filter, ct);
            if (!string.IsNullOrWhiteSpace(categories)) labels.Add($"Categorías: {categories}");

            return labels;
        }

        private async Task<string?> BuildBrandLabelAsync(BrochureFilterDto filter, CancellationToken ct)
        {
            if (filter.BrandIds.Count == 0) return null;
            var brands = await _brandRepo.ListAsync(new BrandsByIdsSpec(filter.BrandIds), ct);
            return brands.Count == 0 ? null : string.Join(" · ", brands.Select(b => b.Name));
        }

        private async Task<string?> BuildCategoryLabelAsync(BrochureFilterDto filter, CancellationToken ct)
        {
            if (filter.CategoryIds.Count == 0) return null;
            var categories = await _categoryRepo.ListAsync(new CategoriesByIdsSpec(filter.CategoryIds), ct);
            return categories.Count == 0 ? null : string.Join(" · ", categories.Select(c => c.Name));
        }

        // ── Specs locales ─────────────────────────────────────────────────────

        private sealed class PriceListByIdSpec : Specification<PriceList>
        {
            public PriceListByIdSpec(int id) => Query.Where(x => x.Id == id).AsNoTracking();
        }

        private sealed class BrandsByIdsSpec : Specification<Brand>
        {
            public BrandsByIdsSpec(IReadOnlyCollection<int> ids) =>
                Query.Where(x => ids.Contains(x.Id)).OrderBy(x => x.Name).AsNoTracking();
        }

        private sealed class CategoriesByIdsSpec : Specification<Category>
        {
            public CategoriesByIdsSpec(IReadOnlyCollection<int> ids) =>
                Query.Where(x => ids.Contains(x.Id)).OrderBy(x => x.Name).AsNoTracking();
        }
    }
}
