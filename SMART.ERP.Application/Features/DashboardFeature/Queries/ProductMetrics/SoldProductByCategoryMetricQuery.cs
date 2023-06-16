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
            var opportunities = await _repositoryAsync.ListAsync(new FilterWonOpportunitiesSpecification(null));
            if (request.CategoryId != null)
            {
                opportunities = opportunities.FindAll(x => x.QuoteProducts!.Any(x => x.Product!.SubCategory!.CategoryId == request.CategoryId));
                opportunities.ForEach(x =>
                {
                    x.QuoteProducts = x.QuoteProducts.Where(y => y.Product.SubCategory.CategoryId == request.CategoryId).ToList();
                });
            }
            var response = new List<ProductComparativeSaleDto>();
            var lastYear = opportunities.FindAll(x => x.ClosingDate.HasValue && x.ClosingDate.Value.Year == DateTime.Now.AddYears(-1).Year);
            foreach (var opportunity in lastYear)
            {
                foreach (var quote in opportunity.QuoteProducts!)
                {
                    if (!response.Exists(x => x.Product == quote.Product.Name))
                    {
                        var dto = new ProductComparativeSaleDto
                        {
                            Product = quote.Product.Name,
                            LastYear = quote.Quantity
                        };
                        response.Add(dto);
                    }
                    else
                    {
                        var index = response.FindIndex(x => x.Product == quote.Product.Name);
                        response[index].LastYear += quote.Quantity;
                    }
                }
            }
            var currentYear = opportunities.FindAll(x => x.ClosingDate.HasValue && x.ClosingDate.Value.Year == DateTime.Now.Year);
            foreach (var opportunity in currentYear)
            {
                foreach (var quote in opportunity.QuoteProducts!)
                {
                    if (!response.Exists(x => x.Product == quote.Product.Name))
                    {
                        var dto = new ProductComparativeSaleDto
                        {
                            Product = quote.Product.Name,
                            CurrentYear = quote.Quantity
                        };
                        response.Add(dto);
                    }
                    else
                    {
                        var index = response.FindIndex(x => x.Product == quote.Product.Name);
                        response[index].CurrentYear += quote.Quantity;
                    }
                }
            }
            return new Response<List<ProductComparativeSaleDto>>(response);
        }
    }
}
