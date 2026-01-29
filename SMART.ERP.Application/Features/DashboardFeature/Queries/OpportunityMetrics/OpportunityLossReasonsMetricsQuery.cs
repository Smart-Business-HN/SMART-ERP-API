using MediatR;
using SMART.ERP.Application.DTOs.Dashboard;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.OpportunitySpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.DashboardFeature.Queries.OpportunityMetrics
{
    public class OpportunityLossReasonsMetricsQuery : IRequest<Response<List<OpportunityReasonsMetricsDto>>>
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int BranchOfficeId { get; set; }
    }

    public class OpportunityLossReasonsMetricsQueryHandler : IRequestHandler<OpportunityLossReasonsMetricsQuery, Response<List<OpportunityReasonsMetricsDto>>>
    {
        private readonly IRepositoryAsync<Opportunity> _repositoryAsync;
        private readonly IRepositoryAsync<LossReason> _lossReasonRepositoryAsync;

        public OpportunityLossReasonsMetricsQueryHandler(IRepositoryAsync<Opportunity> repositoryAsync, IRepositoryAsync<LossReason> lossReasonRepositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
            _lossReasonRepositoryAsync = lossReasonRepositoryAsync;
        }

        public async Task<Response<List<OpportunityReasonsMetricsDto>>> Handle(OpportunityLossReasonsMetricsQuery request, CancellationToken cancellationToken)
        {
            var lossReasons = await _lossReasonRepositoryAsync.ListAsync();
            var lostOpportunities = new List<Opportunity>();
            lostOpportunities = await _repositoryAsync.ListAsync(new FilterClosedOpportunitiesinDatesSpecification(request.StartDate, request.EndDate, null, 0, request.BranchOfficeId));
            var response = new List<OpportunityReasonsMetricsDto>();
            foreach (var loss in lossReasons)
            {
                var countLoss = lostOpportunities.FindAll(x => x.LossReasonId == loss.Id);
                if (countLoss.Count > 0)
                {
                    var dto = new OpportunityReasonsMetricsDto();
                    dto.Name = loss.Name;
                    dto.Quantity = countLoss.Count;
                    dto.Total = countLoss.Sum(a => a.Total);
                    response.Add(dto);
                }

            }
            return new Response<List<OpportunityReasonsMetricsDto>>(response);

        }
    }
}
