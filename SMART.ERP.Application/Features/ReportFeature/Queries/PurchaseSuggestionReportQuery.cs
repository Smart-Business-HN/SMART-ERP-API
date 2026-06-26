using MediatR;
using SMART.ERP.Application.DTOs.Report;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.CategorySpecification;
using SMART.ERP.Application.Specifications.ReportSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.ReportFeature.Queries
{
    /// <summary>
    /// Reporte de sugerencias de compra para abastecimiento de inventario.
    /// Cruza las unidades vendidas (velocidad de venta) contra el stock actual de cada
    /// producto que toca inventario y sugiere cuanto comprar para cubrir N dias de venta.
    /// </summary>
    public class PurchaseSuggestionReportQuery : IRequest<PagedResponse<List<PurchaseSuggestionDto>>>
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int CoverageDays { get; set; } = 30;
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public bool All { get; set; }
    }

    public class PurchaseSuggestionReportQueryHandler : IRequestHandler<PurchaseSuggestionReportQuery, PagedResponse<List<PurchaseSuggestionDto>>>
    {
        private readonly IReadRepositoryAsync<Product> _productRepository;
        private readonly IReadRepositoryAsync<ProductSold> _productSoldRepository;
        private readonly IReadRepositoryAsync<Category> _categoryRepository;

        public PurchaseSuggestionReportQueryHandler(
            IReadRepositoryAsync<Product> productRepository,
            IReadRepositoryAsync<ProductSold> productSoldRepository,
            IReadRepositoryAsync<Category> categoryRepository)
        {
            _productRepository = productRepository;
            _productSoldRepository = productSoldRepository;
            _categoryRepository = categoryRepository;
        }

        public async Task<PagedResponse<List<PurchaseSuggestionDto>>> Handle(PurchaseSuggestionReportQuery request, CancellationToken cancellationToken)
        {
            if (request.StartDate != null && request.EndDate != null && request.EndDate < request.StartDate)
            {
                throw new ApiException("La fecha final debe ser mayor a la fecha inicial");
            }

            var coverageDays = request.CoverageDays <= 0 ? 30 : request.CoverageDays;

            // Dias del periodo de ventas (inclusive). Sin rango, se usan los dias de cobertura como base.
            decimal periodDays = coverageDays;
            if (request.StartDate != null && request.EndDate != null)
            {
                periodDays = (decimal)Math.Max(1, (request.EndDate.Value.Date - request.StartDate.Value.Date).TotalDays + 1);
            }

            // Unidades vendidas por producto en el rango.
            var soldLines = await _productSoldRepository.ListAsync(new SoldUnitsByDateSpecification(request.StartDate, request.EndDate), cancellationToken);
            var unitsByProduct = soldLines
                .Where(x => x.ProductId != null)
                .GroupBy(x => x.ProductId!.Value)
                .ToDictionary(g => g.Key, g => g.Sum(x => x.Quantity));

            var categories = await _categoryRepository.ListAsync(new IncludeCategorySpecification(), cancellationToken);
            var products = await _productRepository.ListAsync(new PurchaseSuggestionProductsSpecification(), cancellationToken);

            var response = new List<PurchaseSuggestionDto>();
            foreach (var product in products)
            {
                unitsByProduct.TryGetValue(product.Id, out var unitsSold);

                var averageDailySales = unitsSold / periodDays;
                var targetStock = averageDailySales * coverageDays;
                var suggestedQuantity = (int)Math.Ceiling(Math.Max(0m, targetStock - product.CurrentStock));
                decimal? daysOfCoverage = averageDailySales > 0 ? product.CurrentStock / averageDailySales : null;

                var category = categories.Find(x => x.Subcategories.Exists(y => y.Id == product.SubCategoryId));

                response.Add(new PurchaseSuggestionDto
                {
                    ProductId = product.Id,
                    Code = product.Code,
                    Name = product.Name,
                    Category = category?.Name ?? string.Empty,
                    SubCategory = product.SubCategory?.Name ?? string.Empty,
                    ProviderId = product.ProviderId,
                    SupplierName = product.Provider?.Name ?? string.Empty,
                    UnitOfMeasurement = product.UnitOfMeasurement?.Abreviation ?? string.Empty,
                    UnitsSold = unitsSold,
                    AverageDailySales = Math.Round(averageDailySales, 2),
                    CurrentStock = product.CurrentStock,
                    MinStock = product.MinStock,
                    DaysOfCoverage = daysOfCoverage.HasValue ? Math.Round(daysOfCoverage.Value, 1) : null,
                    CoverageDays = coverageDays,
                    SuggestedQuantity = suggestedQuantity,
                    CostPrice = product.CostPrice,
                    EstimatedCost = suggestedQuantity * product.CostPrice
                });
            }

            response = response
                .OrderByDescending(x => x.SuggestedQuantity)
                .ThenBy(x => x.SupplierName)
                .ThenByDescending(x => x.UnitsSold)
                .ToList();

            if (request.All)
            {
                request.PageNumber = 0;
                request.PageSize = response.Count;
            }

            var pagedResponse = response.Skip(request.PageNumber * request.PageSize).Take(request.PageSize).ToList();
            return new PagedResponse<List<PurchaseSuggestionDto>>(pagedResponse, request.PageNumber, request.PageSize, response.Count);
        }
    }
}
