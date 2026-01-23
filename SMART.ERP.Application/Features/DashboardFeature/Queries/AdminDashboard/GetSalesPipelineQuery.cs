using MediatR;
using SMART.ERP.Application.DTOs.Dashboard;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.OpportunitySpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.DashboardFeature.Queries.AdminDashboard
{
    public class GetSalesPipelineQuery : IRequest<Response<SalesPipelineDto>>
    {
    }

    public class GetSalesPipelineQueryHandler : IRequestHandler<GetSalesPipelineQuery, Response<SalesPipelineDto>>
    {
        private readonly IRepositoryAsync<Opportunity> _opportunityRepository;
        private readonly IRepositoryAsync<OpportunityStep> _stepRepository;

        public GetSalesPipelineQueryHandler(
            IRepositoryAsync<Opportunity> opportunityRepository,
            IRepositoryAsync<OpportunityStep> stepRepository)
        {
            _opportunityRepository = opportunityRepository;
            _stepRepository = stepRepository;
        }

        public async Task<Response<SalesPipelineDto>> Handle(GetSalesPipelineQuery request, CancellationToken cancellationToken)
        {
            var activeOpportunities = await _opportunityRepository.ListAsync(
                new FilterActiveOpportunitiesWithStepSpecification(), cancellationToken);
            var steps = await _stepRepository.ListAsync(cancellationToken);

            var result = new SalesPipelineDto();

            foreach (var step in steps.OrderBy(s => s.Id))
            {
                var stepOpportunities = activeOpportunities
                    .Where(o => o.OpportunityStepId == step.Id)
                    .ToList();

                var avgProbability = stepOpportunities.Any()
                    ? stepOpportunities.Average(o => o.ProbabilityPercentage)
                    : 0;

                result.Stages.Add(new PipelineStageDto
                {
                    StepId = step.Id,
                    StepName = step.Name,
                    OpportunityCount = stepOpportunities.Count,
                    TotalBudget = stepOpportunities.Sum(o => o.Budget),
                    AverageProbability = Math.Round((decimal)avgProbability, 1),
                    WeightedValue = stepOpportunities.Sum(o => o.Budget * ((decimal)o.ProbabilityPercentage / 100m))
                });
            }

            result.TotalPipelineValue = result.Stages.Sum(s => s.TotalBudget);
            result.WeightedPipelineValue = result.Stages.Sum(s => s.WeightedValue);
            result.TotalOpportunities = result.Stages.Sum(s => s.OpportunityCount);

            return new Response<SalesPipelineDto>(result);
        }
    }
}
