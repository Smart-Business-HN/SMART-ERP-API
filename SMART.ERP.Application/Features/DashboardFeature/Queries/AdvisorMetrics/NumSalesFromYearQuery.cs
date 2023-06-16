using MediatR;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.OpportunitySpecification;
using SMART.ERP.Application.Specifications.UserSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Application.DTOs.Dashboard;

namespace SMART.ERP.Application.Features.DashboardFeature.Queries.AdvisorMetrics
{
    public class NumSalesFromYearQuery : IRequest<Response<List<NumSalesByAdvisorDto>>>
    {
        public int Year { get; set; }
        public int BranchOfficeId { get; set; }
    }
    public class NumSalesFromYearQueryHandler : IRequestHandler<NumSalesFromYearQuery, Response<List<NumSalesByAdvisorDto>>>
    {
        private readonly IRepositoryAsync<User> _repositoryAsync;
        private readonly IRepositoryAsync<Opportunity> _opportunityRepositoryAsync;

        public NumSalesFromYearQueryHandler(IRepositoryAsync<User> repositoryAsync, IRepositoryAsync<Opportunity> opportunityRepositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
            _opportunityRepositoryAsync = opportunityRepositoryAsync;
        }

        public async Task<Response<List<NumSalesByAdvisorDto>>> Handle(NumSalesFromYearQuery request, CancellationToken cancellationToken)
        {
            var response = new List<NumSalesByAdvisorDto>();
            var salesAdvisors = new List<User>();
            salesAdvisors = await _repositoryAsync.ListAsync(new FilterUserByRoleSpecification("Sales Advisor", request.BranchOfficeId));

            foreach (var user in salesAdvisors)
            {
                var dto = new NumSalesByAdvisorDto();
                dto.FullName = user.FullName;
                var sales = await _opportunityRepositoryAsync.ListAsync(new FilterClosedOpportunitiesInYearByUserSpecification(request.Year, user.Id));
                sales = sales.FindAll(x => x.OpportunityStep.Name == "Ganado");
                dto.NumSales = sales.Count;
                dto.Total = sales.Sum(a => a.Total);
                response.Add(dto);
            }
            return new Response<List<NumSalesByAdvisorDto>>(response);
        }
    }
}
