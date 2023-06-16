using MediatR;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.CategorySpecification;
using SMART.ERP.Application.Specifications.OpportunitySpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Application.DTOs.Dashboard;
using System.Globalization;

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
            var productsMonth = new List<MonthlyProductDto>();
            if (request.Date != null)
            {
                var categories = await _categoryRepositoryAsync.ListAsync(new IncludeCategorySpecification());
                var doneOpportunitiesFromMonth = await _repositoryAsync.ListAsync(new FilterOpportunityFromDateSpecification((DateTime)request.Date, request.Time, request.BranchOfficeId));
                if (request.Time.ToLower() == "semana" && doneOpportunitiesFromMonth.Count > 0)
                {
                    var cal = CultureInfo.InvariantCulture.Calendar;
                    doneOpportunitiesFromMonth = doneOpportunitiesFromMonth.FindAll(x => cal.GetWeekOfYear((DateTime)x.ClosingDate!, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday) == cal.GetWeekOfYear((DateTime)request.Date, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday));
                }
                foreach (var category in categories)
                {
                    var dto = new MonthlyProductDto();
                    dto.Name = category.Name;
                    dto.SoldProducts = 0;
                    var filteredOpportunities = doneOpportunitiesFromMonth.Where(x => x.QuoteProducts!.Exists(y => y.Product!.SubCategory!.Name == category.Name));
                    foreach (var opportunity in filteredOpportunities)
                    {
                        foreach (var product in opportunity.QuoteProducts!)
                        {
                            if (category.Subcategories.Any(a => a.Name == product.Product!.SubCategory!.Name))
                            {
                                dto.SoldProducts += product.Quantity;
                            }
                        }
                    }
                    productsMonth.Add(dto);
                }
                return new Response<List<MonthlyProductDto>>(productsMonth);
            }
            else
            {
                var categories = await _categoryRepositoryAsync.ListAsync(new IncludeCategorySpecification());
                var doneOpportunitiesFromMonth = await _repositoryAsync.ListAsync(new FilterOpportunityFromDateSpecification(DateTime.Now, request.Time, request.BranchOfficeId));
                foreach (var category in categories)
                {
                    var dto = new MonthlyProductDto();
                    dto.Name = category.Name;
                    dto.SoldProducts = 0;
                    var filteredOpportunities = doneOpportunitiesFromMonth.Where(x => x.QuoteProducts!.Exists(y => y.Product!.SubCategory!.Name == category.Name)); // Revisar
                    foreach (var opportunity in filteredOpportunities)
                    {
                        foreach (var product in opportunity.QuoteProducts!)
                        {
                            if (category.Subcategories.Any(a => a.Name == product.Product!.SubCategory!.Name))
                            {
                                dto.SoldProducts += product.Quantity;
                            }
                        }
                    }
                    productsMonth.Add(dto);
                }
                return new Response<List<MonthlyProductDto>>(productsMonth);
            }
        }
    }
}
