using MediatR;
using SMART.ERP.Application.DTOs.Dashboard;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.CategorySpecification;
using SMART.ERP.Application.Specifications.OpportunitySpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.DashboardFeature.Queries
{
    public class ProductsByDateQuery : IRequest<Response<List<MonthlyProductDto>>>
    {
        public DateTime? Date { get; set; }
        public string Time { get; set; } = null!;
        public int BranchOfficeId { get; set; }
    }

    public class ProductsByDateQueryHandler : IRequestHandler<ProductsByDateQuery, Response<List<MonthlyProductDto>>>
    {
        private readonly IRepositoryAsync<Opportunity> _repositoryAsync;
        private readonly IRepositoryAsync<Category> _categoryRepositoryAsync;

        public ProductsByDateQueryHandler(IRepositoryAsync<Opportunity> repositoryAsync, IRepositoryAsync<Category> categoryRepositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
            _categoryRepositoryAsync = categoryRepositoryAsync;
        }

        public async Task<Response<List<MonthlyProductDto>>> Handle(ProductsByDateQuery request, CancellationToken cancellationToken)
        {
            var targetDate = request.Date ?? DateTime.Now;

            // ✓ Optimización: Cargar categorías con subcategorías en una sola query
            var categories = await _categoryRepositoryAsync.ListAsync(
                new IncludeCategorySpecification(),
                cancellationToken
            );

            // ✓ Optimización: Obtener oportunidades con filtrado en SQL usando Specification optimizada
            var closedOpportunities = await _repositoryAsync.ListAsync(
                new FilterClosedOpportunitiesWithProductsSpecification(
                    targetDate,
                    request.Time,
                    request.BranchOfficeId
                ),
                cancellationToken
            );

            // ✓ Optimización: Si es filtro por semana, aplicar filtro adicional en memoria
            if (request.Time.ToLower() == "semana" && closedOpportunities.Count > 0)
            {
                closedOpportunities = FilterClosedOpportunitiesWithProductsSpecification
                    .FilterByWeek(closedOpportunities, targetDate);
            }

            // ✓ Optimización: Crear diccionario de subcategorías para búsqueda O(1)
            var subcategoryToCategoryMap = new Dictionary<string, string>();
            foreach (var category in categories)
            {
                foreach (var subcategory in category.Subcategories)
                {
                    subcategoryToCategoryMap[subcategory.Name] = category.Name;
                }
            }

            // ✓ Optimización: Usar Dictionary para agrupación por categoría O(n)
            var categorySalesDict = new Dictionary<string, int>();

            // Inicializar todas las categorías con 0
            foreach (var category in categories)
            {
                categorySalesDict[category.Name] = 0;
            }

            // ✓ Optimización: Procesar en un solo recorrido
            foreach (var opportunity in closedOpportunities)
            {
                if (opportunity.QuoteProducts == null) continue;

                foreach (var product in opportunity.QuoteProducts)
                {
                    var subcategoryName = product.Product?.SubCategory?.Name;
                    if (subcategoryName != null && subcategoryToCategoryMap.TryGetValue(subcategoryName, out var categoryName))
                    {
                        categorySalesDict[categoryName] += product.Quantity;
                    }
                }
            }

            // ✓ Convertir Dictionary a List de DTOs
            var response = categorySalesDict
                .Select(kvp => new MonthlyProductDto
                {
                    Name = kvp.Key,
                    SoldProducts = kvp.Value
                })
                .OrderByDescending(x => x.SoldProducts)
                .ToList();

            return new Response<List<MonthlyProductDto>>(response);
        }
    }
}
