using MediatR;
using SMART.ERP.Application.DTOs.Product;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services;
using SMART.ERP.Application.Specifications.ProductSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.BaseProductFeature.Queries
{
    public class GetSearchSuggestionsQuery : IRequest<Response<List<ProductSuggestionDto>>>
    {
        public string SearchTerm { get; set; } = null!;
        public int Limit { get; set; } = 10;
    }

    public class GetSearchSuggestionsQueryHandler : IRequestHandler<GetSearchSuggestionsQuery, Response<List<ProductSuggestionDto>>>
    {
        private readonly IRepositoryAsync<Product> _repositoryAsync;
        private readonly IProductPricingService _productPricingService;

        public GetSearchSuggestionsQueryHandler(
            IRepositoryAsync<Product> repositoryAsync,
            IProductPricingService productPricingService)
        {
            _repositoryAsync = repositoryAsync;
            _productPricingService = productPricingService;
        }

        public async Task<Response<List<ProductSuggestionDto>>> Handle(GetSearchSuggestionsQuery request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.SearchTerm) || request.SearchTerm.Length < 2)
            {
                return new Response<List<ProductSuggestionDto>>(new List<ProductSuggestionDto>());
            }

            var products = await _repositoryAsync.ListAsync(
                new ProductSearchSuggestionsSpecification(request.SearchTerm, request.Limit));

            // Precio público (lista de precios + impuesto) consistente con la búsqueda y el detalle
            // del producto. El endpoint es anónimo y cacheado globalmente, así que se resuelve el
            // precio para usuario no autenticado (no por tipo de cliente).
            var prices = await _productPricingService.CalculateRecommendedSalePricesAsync(
                products.Select(p => p.Id),
                isUserSignedIn: false,
                customerTypeId: null,
                ct: cancellationToken);

            var suggestions = products.Select(p => new ProductSuggestionDto
            {
                Id = p.Id,
                Code = p.Code,
                Name = p.Name,
                Slug = p.Slug,
                Thumbnail = p.ProductImages != null && p.ProductImages.Count > 0 ? p.ProductImages[0].Url : null,
                BrandName = p.Brand?.Name,
                Price = prices.GetValueOrDefault(p.Id, p.RecomendedSalePrice),
                SubCategorySlug = p.SubCategory?.Slug,
                CategorySlug = p.SubCategory?.Category?.Slug
            }).ToList();

            return new Response<List<ProductSuggestionDto>>(suggestions);
        }
    }
}
