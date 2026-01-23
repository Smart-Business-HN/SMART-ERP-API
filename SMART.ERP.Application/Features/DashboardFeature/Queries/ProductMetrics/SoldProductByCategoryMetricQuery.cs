using MediatR;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.OpportunitySpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Application.DTOs.Dashboard;

namespace SMART.ERP.Application.Features.DashboardFeature.Queries.ProductMetrics
{
    public class SoldProductByCategoryMetricQuery : IRequest<Response<List<ProductComparativeSaleDto>>>
    {
        public int? CategoryId { get; set; }
    }

    public class SoldProductByCategoryMetricQueryHandler : IRequestHandler<SoldProductByCategoryMetricQuery, Response<List<ProductComparativeSaleDto>>>
    {
        private readonly IRepositoryAsync<Opportunity> _repositoryAsync;

        public SoldProductByCategoryMetricQueryHandler(IRepositoryAsync<Opportunity> repositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
        }

        public async Task<Response<List<ProductComparativeSaleDto>>> Handle(SoldProductByCategoryMetricQuery request, CancellationToken cancellationToken)
        {
            // ✓ Optimización: Obtener oportunidades del año pasado con filtrado en SQL
            var lastYearOpportunities = await _repositoryAsync.ListAsync(
                new FilterWonOpportunitiesByYearAndCategorySpecification(
                    DateTime.Now.AddYears(-1).Year,
                    request.CategoryId
                ),
                cancellationToken
            );

            // ✓ Optimización: Obtener oportunidades del año actual con filtrado en SQL
            var currentYearOpportunities = await _repositoryAsync.ListAsync(
                new FilterWonOpportunitiesByYearAndCategorySpecification(
                    DateTime.Now.Year,
                    request.CategoryId
                ),
                cancellationToken
            );

            // ✓ Optimización: Usar Dictionary para agrupación O(n) en lugar de List.FindIndex O(n²)
            var productSalesDict = new Dictionary<string, ProductComparativeSaleDto>();

            // Procesar oportunidades del año pasado
            foreach (var opportunity in lastYearOpportunities)
            {
                foreach (var quote in opportunity.QuoteProducts!)
                {
                    // Filtrar por categoría si se especificó (aunque ya viene filtrado del SQL)
                    if (request.CategoryId.HasValue &&
                        quote.Product?.SubCategory?.CategoryId != request.CategoryId.Value)
                        continue;

                    var productName = quote.Product!.Name;

                    if (!productSalesDict.ContainsKey(productName))
                    {
                        productSalesDict[productName] = new ProductComparativeSaleDto
                        {
                            Product = productName,
                            LastYear = quote.Quantity,
                            CurrentYear = 0
                        };
                    }
                    else
                    {
                        productSalesDict[productName].LastYear += quote.Quantity;
                    }
                }
            }

            // Procesar oportunidades del año actual
            foreach (var opportunity in currentYearOpportunities)
            {
                foreach (var quote in opportunity.QuoteProducts!)
                {
                    // Filtrar por categoría si se especificó (aunque ya viene filtrado del SQL)
                    if (request.CategoryId.HasValue &&
                        quote.Product?.SubCategory?.CategoryId != request.CategoryId.Value)
                        continue;

                    var productName = quote.Product!.Name;

                    if (!productSalesDict.ContainsKey(productName))
                    {
                        productSalesDict[productName] = new ProductComparativeSaleDto
                        {
                            Product = productName,
                            LastYear = 0,
                            CurrentYear = quote.Quantity
                        };
                    }
                    else
                    {
                        productSalesDict[productName].CurrentYear += quote.Quantity;
                    }
                }
            }

            // ✓ Convertir Dictionary a List y ordenar por total de ventas
            var response = productSalesDict.Values
                .OrderByDescending(x => x.LastYear + x.CurrentYear)
                .ToList();

            return new Response<List<ProductComparativeSaleDto>>(response);
        }
    }
}
