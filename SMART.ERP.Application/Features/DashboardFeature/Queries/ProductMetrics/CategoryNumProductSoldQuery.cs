using MediatR;
using SMART.ERP.Application.DTOs.Dashboard;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.OpportunitySpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.DashboardFeature.Queries.ProductMetrics
{
    public class CategoryNumProductSoldQuery : IRequest<Response<List<CategorySaleMetricDto>>>
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int BranchOfficeId { get; set; }
        public int? OpportunityStepId { get; set; }
    }

    public class CategoryNumProductSoldQueryHandler : IRequestHandler<CategoryNumProductSoldQuery, Response<List<CategorySaleMetricDto>>>
    {
        private readonly IRepositoryAsync<Opportunity> _repositoryAsync;
        private readonly IRepositoryAsync<Category> _categoryRepositoryAsync;

        public CategoryNumProductSoldQueryHandler(IRepositoryAsync<Opportunity> repositoryAsync, IRepositoryAsync<Category> categoryRepositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
            _categoryRepositoryAsync = categoryRepositoryAsync;
        }

        public async Task<Response<List<CategorySaleMetricDto>>> Handle(CategoryNumProductSoldQuery request, CancellationToken cancellationToken)
        {
            if (request.OpportunityStepId == 0)
            {
                request.OpportunityStepId = null;
            }

            var response = new List<CategorySaleMetricDto>();
            var categories = await _categoryRepositoryAsync.ListAsync();
            var sales = new List<Opportunity>();
            sales = await _repositoryAsync.ListAsync(new FilterOpportunitiesInStepSpecification(request.OpportunityStepId, null, null, request.BranchOfficeId));
            if (request.OpportunityStepId >= 7)
            {
                if (request.StartDate != null)
                {
                    sales = sales.FindAll(x => x.ClosingDate.HasValue && x.ClosingDate.Value.Date >= request.StartDate!.Value.Date);
                }
                if (request.EndDate != null)
                {
                    sales = sales.FindAll(x => x.ClosingDate.HasValue && x.ClosingDate.Value.Date <= request.EndDate!.Value.Date);
                }
            }
            else
            {
                if (request.OpportunityStepId == null)
                {
                    var notClosed = new List<Opportunity>();
                    var closed = new List<Opportunity>();
                    if (request.StartDate != null)
                    {
                        notClosed = sales.FindAll(x => x.CreationDate.Date >= request.StartDate.Value.Date);
                        closed = sales.FindAll(x => x.ClosingDate.HasValue && x.ClosingDate.Value.Date >= request.StartDate);
                    }
                    if (request.EndDate != null)
                    {
                        if (request.StartDate != null)
                        {
                            notClosed = notClosed.FindAll(x => x.CreationDate.Date <= request.EndDate.Value.Date);
                            closed = closed.FindAll(x => x.ClosingDate.HasValue && x.ClosingDate.Value.Date <= request.EndDate);
                        }
                        else
                        {
                            notClosed = sales.FindAll(x => x.CreationDate.Date <= request.EndDate.Value.Date);
                            closed = sales.FindAll(x => x.ClosingDate.HasValue && x.ClosingDate.Value.Date <= request.EndDate);
                        }
                    }
                    var unifiedList = closed.Concat(notClosed).ToList();
                    var hashSet = new HashSet<Opportunity>(unifiedList);
                    sales = hashSet.ToList();
                }
                else
                {
                    if (request.StartDate != null)
                    {
                        sales = sales.FindAll(x => x.CreationDate.Date >= request.StartDate!.Value.Date);
                    }
                    if (request.EndDate != null)
                    {
                        sales = sales.FindAll(x => x.CreationDate.Date <= request.EndDate!.Value.Date);
                    }
                }
            }

            foreach (var category in categories)
            {
                var dto = new CategorySaleMetricDto();
                dto.Name = category.Name;
                dto.Quantity = 0;
                var opportunities = sales.FindAll(x => x.QuoteProducts!.Exists(x => x.Product!.SubCategory!.CategoryId == category.Id));
                foreach (var opportunity in opportunities)
                {
                    foreach (var quote in opportunity.QuoteProducts!)
                    {
                        if (quote.Product!.SubCategory!.CategoryId == category.Id)
                        {
                            dto.Quantity += quote.Quantity;
                            dto.Total += quote.Quantity * quote.SalePrice;
                        }
                    }
                }
                response.Add(dto);
            }
            return new Response<List<CategorySaleMetricDto>>(response);
        }
    }
}
