using AutoMapper;
using MediatR;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.OpportunitySpecification;
using SMART.ERP.Application.Specifications.UserSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Application.DTOs.Dashboard;

namespace SMART.ERP.Application.Features.DashboardFeature.Queries.AdvisorMetrics
{
    public class SalesFromYearQuery : IRequest<Response<List<SalesByAdvisorDto>>>
    {
        public int Year { get; set; }
        public int BranchOfficeId { get; set; }
    }

    public class SalesFromYearQueryHandler : IRequestHandler<SalesFromYearQuery, Response<List<SalesByAdvisorDto>>>
    {
        private readonly IRepositoryAsync<User> _repositoryAsync;
        private readonly IRepositoryAsync<Opportunity> _opportunityRepositoryAsync;
        private readonly IMapper _mapper;

        public SalesFromYearQueryHandler(IRepositoryAsync<User> repositoryAsync, IRepositoryAsync<Opportunity> opportunityRepositoryAsync, IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _opportunityRepositoryAsync = opportunityRepositoryAsync;
            _mapper = mapper;
        }

        public async Task<Response<List<SalesByAdvisorDto>>> Handle(SalesFromYearQuery request, CancellationToken cancellationToken)
        {

            string[] months = { "Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio", "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre" };
            var response = new List<SalesByAdvisorDto>();
            var salesAdvisors = new List<User>();
            salesAdvisors = await _repositoryAsync.ListAsync(new FilterUserByRoleSpecification("Sales Advisor", request.BranchOfficeId));

            foreach (var user in salesAdvisors)
            {
                var dto = new SalesByAdvisorDto();
                dto = _mapper.Map<SalesByAdvisorDto>(user);
                var sales = await _opportunityRepositoryAsync.ListAsync(new FilterClosedOpportunitiesInYearByUserSpecification(request.Year, user.Id));
                sales = sales.FindAll(x => x.OpportunityStep.Name == "Ganado");
                foreach (var month in months)
                {
                    foreach (var sale in sales)
                    {
                        if (sale.ClosingDate!.Value.Month == Array.IndexOf(months, month) + 1)
                        {
                            dto.Data[sale.ClosingDate.Value.Month - 1] += sale.Total;
                        }
                    }
                }
                response.Add(dto);

            }
            return new Response<List<SalesByAdvisorDto>>(response);
        }
    }
}
