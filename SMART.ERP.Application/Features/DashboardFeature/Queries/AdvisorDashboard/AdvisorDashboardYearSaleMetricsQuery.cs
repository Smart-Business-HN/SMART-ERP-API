using MediatR;
using SMART.ERP.Application.DTOs.Dashboard;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.JwtService;
using SMART.ERP.Application.Specifications.AdvisorGoalSpecification;
using SMART.ERP.Application.Specifications.InvoiceSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.DashboardFeature.Queries.AdvisorDashboard
{
    public class AdvisorDashboardYearSaleMetricsQuery : IRequest<Response<List<AdvisorGoalMetricDto>>>
    {
        public int Year { get; set; }
    }

    public class AdvisorDashboardYearSaleMetricsQueryHandler : IRequestHandler<AdvisorDashboardYearSaleMetricsQuery, Response<List<AdvisorGoalMetricDto>>>
    {
        private readonly IRepositoryAsync<AdvisorGoal> _advisorGoalRepositoryAsync;
        private readonly IRepositoryAsync<Invoice> _invoiceRepositoryAsync;
        private readonly IJwtService _jwtService;

        public AdvisorDashboardYearSaleMetricsQueryHandler(IRepositoryAsync<AdvisorGoal> advisorGoalRepositoryAsync,
            IRepositoryAsync<Invoice> invoiceRepositoryAsync, IJwtService jwtService)
        {
            _advisorGoalRepositoryAsync = advisorGoalRepositoryAsync;
            _invoiceRepositoryAsync = invoiceRepositoryAsync;
            _jwtService = jwtService;
        }

        public async Task<Response<List<AdvisorGoalMetricDto>>> Handle(AdvisorDashboardYearSaleMetricsQuery request, CancellationToken cancellationToken)
        {
            Guid guid = _jwtService.GetUidToken();
            //Advisor Year Goal Metric
            string[] months = { "Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio", "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre" };
            var yearAdvisorGoals = await _advisorGoalRepositoryAsync.ListAsync(new FilterAdvisorGoalByYearSpecification(request.Year, guid));
            var response = new List<AdvisorGoalMetricDto>();
            var closedOppotunitiesinYear = await _invoiceRepositoryAsync.ListAsync(new FilterInvoiceByYearAndUserIdSpecification(request.Year, guid));
            for (int i = 0; i < months.Length; i++)
            {
                var metricDto = new AdvisorGoalMetricDto();
                metricDto.Id = guid;
                metricDto.FullName = months[i];
                var checkMonthGoal = yearAdvisorGoals.FirstOrDefault(x => x.InitDate.Month == i + 1 && x.InitDate.Year == request.Year);
                if (checkMonthGoal != null)
                {
                    metricDto.SalesGoal = checkMonthGoal.Goal;
                }
                else
                {
                    metricDto.SalesGoal = 0;
                }
                metricDto.Total = 0m;
                var closedOpportunitiesinMonth = closedOppotunitiesinYear.FindAll(x => x.CreationDate.Month == i + 1);
                foreach (var opportunity in closedOpportunitiesinMonth)
                {
                    metricDto.Total += opportunity.Total;
                }
                response.Add(metricDto);
            }
            return new Response<List<AdvisorGoalMetricDto>>(response);
        }
    }
}
