using MediatR;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.ProductSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.BaseProductFeature.Queries
{
    public class GetSearchSuggestionsQuery : IRequest<Response<List<string>>>
    {
        public string SearchTerm { get; set; } = null!;
        public int Limit { get; set; } = 10;
    }

    public class GetSearchSuggestionsQueryHandler : IRequestHandler<GetSearchSuggestionsQuery, Response<List<string>>>
    {
        private readonly IRepositoryAsync<Product> _repositoryAsync;

        public GetSearchSuggestionsQueryHandler(IRepositoryAsync<Product> repositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
        }

        public async Task<Response<List<string>>> Handle(GetSearchSuggestionsQuery request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.SearchTerm) || request.SearchTerm.Length < 2)
            {
                return new Response<List<string>>(new List<string>());
            }

            var suggestions = new List<string>();
            var searchTermLower = request.SearchTerm.ToLower();

            // Obtener sugerencias de nombres de productos
            var productNames = await _repositoryAsync.ListAsync(new ProductSearchSuggestionsSpecification(
                searchTermLower, 
                "Name", 
                request.Limit));

            // Obtener sugerencias de marcas
            var brandNames = await _repositoryAsync.ListAsync(new ProductSearchSuggestionsSpecification(
                searchTermLower, 
                "Brand", 
                request.Limit));

            // Obtener sugerencias de categorías
            var categoryNames = await _repositoryAsync.ListAsync(new ProductSearchSuggestionsSpecification(
                searchTermLower, 
                "Category", 
                request.Limit));

            // Combinar y ordenar sugerencias
            suggestions.AddRange(productNames.Select(x=> x.Name));
            suggestions.AddRange(brandNames.Select(x=> x.Name));
            suggestions.AddRange(categoryNames.Select(x=> x.Name));

            // Eliminar duplicados y limitar resultados
            var uniqueSuggestions = suggestions
                .Distinct()
                .OrderBy(x => x)
                .Take(request.Limit)
                .ToList();

            return new Response<List<string>>(uniqueSuggestions);
        }
    }
}

