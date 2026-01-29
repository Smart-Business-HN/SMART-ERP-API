using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Dashboard;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.OpportunitySpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.DashboardFeature.Queries.ProductMetrics
{
    public class ProductSoldComparativeQuery : IRequest<Response<List<ProductComparativeDto>>>
    {
        public int Year { get; set; }
        public int? CategoryId { get; set; }
    }

    public class ProductSoldComparativeQueryHandler : IRequestHandler<ProductSoldComparativeQuery, Response<List<ProductComparativeDto>>>
    {
        private readonly IRepositoryAsync<Opportunity> _repositoryAsync;
        private readonly IMapper _mapper;

        public ProductSoldComparativeQueryHandler(IRepositoryAsync<Opportunity> repositoryAsync, IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _mapper = mapper;
        }

        public async Task<Response<List<ProductComparativeDto>>> Handle(ProductSoldComparativeQuery request, CancellationToken cancellationToken)
        {
            var opportunities = await _repositoryAsync.ListAsync(new FilterClosedOpportunitiesInYearByUserSpecification(request.Year, null));
            var pastOpportunities = await _repositoryAsync.ListAsync(new FilterClosedOpportunitiesInYearByUserSpecification(request.Year - 1, null));
            opportunities = opportunities.FindAll(x => x.OpportunityStep!.Name == "Ganado");
            pastOpportunities = pastOpportunities.FindAll(x => x.OpportunityStep!.Name == "Ganado");
            if (request.CategoryId != null)
            {
                opportunities = opportunities.FindAll(x => x.QuoteProducts!.Any(y => y.Product!.SubCategory!.CategoryId == request.CategoryId));
                opportunities.ForEach(x =>
                {
                    x.QuoteProducts = x.QuoteProducts!.Where(y => y.Product!.SubCategory!.CategoryId == request.CategoryId).ToList();
                });
                pastOpportunities = pastOpportunities.FindAll(x => x.QuoteProducts!.Any(y => y.Product!.SubCategory!.CategoryId == request.CategoryId));
                pastOpportunities.ForEach(x =>
                {
                    x.QuoteProducts = x.QuoteProducts!.Where(y => y.Product!.SubCategory!.CategoryId == request.CategoryId).ToList();
                });
            }
            var response = new List<ProductComparativeDto>();
            foreach (var opportunity in pastOpportunities)
            {
                foreach (var quote in opportunity.QuoteProducts!)
                {
                    int index = response.FindIndex(x => x.Product == quote.Product!.Name);
                    if (index == -1)
                    {
                        var dto = new ProductComparativeDto
                        {
                            Product = quote.Product!.Name,
                            SoldPast = quote.Quantity,
                            SoldPastTotal = quote.Quantity * quote.SalePrice
                        };
                        response.Add(dto);
                    }
                    else
                    {
                        response[index].SoldPastTotal += quote.Quantity * quote.SalePrice;
                        response[index].SoldPast += quote.Quantity;
                    }
                }
            }

            foreach (var opportunity in opportunities)
            {
                foreach (var quote in opportunity.QuoteProducts!)
                {
                    int index = response.FindIndex(x => x.Product == quote.Product!.Name);
                    if (index == -1)
                    {
                        var dto = new ProductComparativeDto
                        {
                            Product = quote.Product!.Name,
                            SoldCurrent = quote.Quantity,
                            SoldCurrentTotal = quote.Quantity * quote.SalePrice
                        };
                        response.Add(dto);
                    }
                    else
                    {
                        response[index].SoldCurrentTotal += quote.Quantity * quote.SalePrice;
                        response[index].SoldCurrent += quote.Quantity;
                    }
                }
            }

            return new Response<List<ProductComparativeDto>>(response);
        }
    }
}
