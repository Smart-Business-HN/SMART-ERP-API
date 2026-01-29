using MediatR;
using SMART.ERP.Application.DTOs.Dashboard;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.OpportunitySpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.DashboardFeature.Queries.OpportunityMetrics
{
    public class OpportunityWinReasonsMetricsQuery : IRequest<Response<List<OpportunityReasonsMetricsDto>>>
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int BranchOfficeId { get; set; }
    }

    public class OpportunityWinReasonsMetricsQueryHandler : IRequestHandler<OpportunityWinReasonsMetricsQuery, Response<List<OpportunityReasonsMetricsDto>>>
    {
        private readonly IRepositoryAsync<Opportunity> _repositoryAsync;
        private readonly IRepositoryAsync<WinReason> _winRepositoryAsync;

        public OpportunityWinReasonsMetricsQueryHandler(IRepositoryAsync<Opportunity> repositoryAsync, IRepositoryAsync<WinReason> winRepositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
            _winRepositoryAsync = winRepositoryAsync;
        }

        public async Task<Response<List<OpportunityReasonsMetricsDto>>> Handle(OpportunityWinReasonsMetricsQuery request, CancellationToken cancellationToken)
        {
            var winReasons = await _winRepositoryAsync.ListAsync();
            var wonOpportunities = new List<Opportunity>();
            wonOpportunities = await _repositoryAsync.ListAsync(new FilterClosedOpportunitiesinDatesSpecification(request.StartDate, request.EndDate, null, 1, request.BranchOfficeId));
            var response = new List<OpportunityReasonsMetricsDto>();
            foreach (var win in winReasons)
            {
                var countLoss = wonOpportunities.FindAll(x => x.WinReasonId == win.Id);
                if (countLoss.Count > 0)
                {
                    var dto = new OpportunityReasonsMetricsDto();
                    dto.Name = win.Name;
                    dto.Quantity = countLoss.Count;
                    dto.Total = countLoss.Sum(a => a.Total);
                    response.Add(dto);
                }

            }
            return new Response<List<OpportunityReasonsMetricsDto>>(response);

        }
    }
}
