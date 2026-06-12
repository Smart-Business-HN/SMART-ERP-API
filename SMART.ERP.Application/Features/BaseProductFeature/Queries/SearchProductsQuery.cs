using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Product;
using SMART.ERP.Application.Parameters;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services;
using SMART.ERP.Application.Specifications.InventoryDistributionSpecification;
using SMART.ERP.Application.Specifications.ProductSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.BaseProductFeature.Queries
{
    public class SearchProductsQuery : IRequest<PagedResponse<List<ProductDto>>>
    {
        public ProductSearchParameter SearchParameters { get; set; } = null!;
    }

    public class SearchProductsQueryHandler : IRequestHandler<SearchProductsQuery, PagedResponse<List<ProductDto>>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<Product> _repositoryAsync;
        private readonly IRepositoryAsync<Category> _categoryRepositoryAsync;
        private readonly IRepositoryAsync<InventoryDistribution> _inventoryRepositoryAsync;
        private readonly IProductPricingService _productPricingService;

        public SearchProductsQueryHandler(
            IMapper mapper,
            IRepositoryAsync<Product> repositoryAsync,
            IRepositoryAsync<Category> categoryRepositoryAsync,
            IRepositoryAsync<InventoryDistribution> inventoryRepositoryAsync,
            IProductPricingService productPricingService)
        {
            _mapper = mapper;
            _repositoryAsync = repositoryAsync;
            _categoryRepositoryAsync = categoryRepositoryAsync;
            _inventoryRepositoryAsync = inventoryRepositoryAsync;
            _productPricingService = productPricingService;
        }

        public async Task<PagedResponse<List<ProductDto>>> Handle(SearchProductsQuery request, CancellationToken cancellationToken)
        {
            var searchParams = request.SearchParameters;
            
            if (searchParams.All)
            {
                searchParams.PageNumber = 0;
                searchParams.PageSize = await _repositoryAsync.CountAsync();
            }

            // Obtener categorías para mapeo
            var categories = await _categoryRepositoryAsync.ListAsync();
            
            // Crear especificación optimizada
            var spec = new OptimizedProductSearchSpecification(
                searchParams.SearchTerm,
                searchParams.PageNumber,
                searchParams.PageSize,
                searchParams.Order,
                searchParams.Column,
                searchParams.MinPrice,
                searchParams.MaxPrice,
                searchParams.BrandId,
                searchParams.CategoryId,
                searchParams.SubCategoryId,
                searchParams.InStock
            );

            // Obtener productos
            var products = await _repositoryAsync.ListAsync(spec);

            // Calcular precios en batch usando el servicio (evita N+1)
            var prices = await _productPricingService.CalculateRecommendedSalePricesAsync(
                products.Select(p => p.Id),
                searchParams.IsUserSignIn,
                searchParams.CustomerTypeId,
                ct: cancellationToken);
            foreach (var product in products)
            {
                product.RecomendedSalePrice = prices.GetValueOrDefault(product.Id, 0);
                product.CostPrice = 0;
                product.Tax = null;
            }

            // Mapear a DTOs
            var dto = _mapper.Map<List<ProductDto>>(products);
            
            // Mapear categorías
            foreach (var product in dto)
            {
                product.SubCategory!.Category = _mapper.Map<CategoryDto>(
                    categories.Find(y => y.Id == product.SubCategory.CategoryId));
            }

            // Disponibilidad ecommerce (físico + virtual) sin tocar CurrentStock.
            var distributions = await _inventoryRepositoryAsync.ListAsync(new FilterInventoryByProductIdsSpec(dto.Select(d => d.Id).ToList()));
            ProductAvailabilityHelper.ApplyEcommerceStock(dto, distributions);

            // Contar total de resultados
            var countSpec = new OptimizedProductSearchSpecification(
                searchParams.SearchTerm,
                0, // No paginación para contar
                int.MaxValue, // Obtener todos para contar
                null, // Sin ordenamiento para contar
                null,
                searchParams.MinPrice,
                searchParams.MaxPrice,
                searchParams.BrandId,
                searchParams.CategoryId,
                searchParams.SubCategoryId,
                searchParams.InStock
            );

            var totalCount = await _repositoryAsync.CountAsync(countSpec);

            return new PagedResponse<List<ProductDto>>(
                dto, 
                searchParams.PageNumber, 
                searchParams.PageSize, 
                searchParams.All ? searchParams.PageSize : totalCount);
        }
    }
}

