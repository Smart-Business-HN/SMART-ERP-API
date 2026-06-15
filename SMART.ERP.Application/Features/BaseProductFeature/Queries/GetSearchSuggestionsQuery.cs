using MediatR;
using SMART.ERP.Application.DTOs.Product;
using SMART.ERP.Application.Repository;
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

        public GetSearchSuggestionsQueryHandler(IRepositoryAsync<Product> repositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
        }

        public async Task<Response<List<ProductSuggestionDto>>> Handle(GetSearchSuggestionsQuery request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.SearchTerm) || request.SearchTerm.Length < 2)
            {
                return new Response<List<ProductSuggestionDto>>(new List<ProductSuggestionDto>());
            }

            var products = await _repositoryAsync.ListAsync(
                new ProductSearchSuggestionsSpecification(request.SearchTerm, request.Limit));

            var suggestions = products.Select(p => new ProductSuggestionDto
            {
                Id = p.Id,
                Code = p.Code,
                Name = p.Name,
                Slug = p.Slug,
                Thumbnail = p.ProductImages != null && p.ProductImages.Count > 0 ? p.ProductImages[0].Url : null,
                BrandName = p.Brand?.Name,
                Price = p.RecomendedSalePrice
            }).ToList();

            return new Response<List<ProductSuggestionDto>>(suggestions);
        }
    }
}
