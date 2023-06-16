using MediatR;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.OpportunitySpecification;
using SMART.ERP.Application.Specifications.StatusSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Application.DTOs.Dashboard;

namespace SMART.ERP.Application.Features.DashboardFeature.Queries.AdvisorMetrics
{
    public class PendingActivitiesByAdvisorQuery : IRequest<Response<List<PendingActivitiesDto>>>
    {
        public int BranchOfficeId { get; set; }
    }

    public class PendingActivitiesByAdvisorQueryHandler : IRequestHandler<PendingActivitiesByAdvisorQuery, Response<List<PendingActivitiesDto>>>
    {
        private readonly IRepositoryAsync<Opportunity> _opportunityRepositoryAsync;
        private readonly IRepositoryAsync<Status> _statusRepositoryAsync;

        public PendingActivitiesByAdvisorQueryHandler(IRepositoryAsync<Opportunity> opportunityRepositoryAsync
            , IRepositoryAsync<Status> statusRepositoryAsync)
        {
            _opportunityRepositoryAsync = opportunityRepositoryAsync;
            _statusRepositoryAsync = statusRepositoryAsync;
        }

        public async Task<Response<List<PendingActivitiesDto>>> Handle(PendingActivitiesByAdvisorQuery request, CancellationToken cancellationToken)
        {
            var currentlyActiveOpportunities = await _opportunityRepositoryAsync.ListAsync(new FilterActiveOpportunitiesByBranchOfficeSpecification(request.BranchOfficeId));
            var pendingStatus = await _statusRepositoryAsync.FirstOrDefaultAsync(new FilterStatusFromNameSpecification("En Proceso"));
            var pausedStatus = await _statusRepositoryAsync.FirstOrDefaultAsync(new FilterStatusFromNameSpecification("Pausado"));
            var finishedStatus = await _statusRepositoryAsync.FirstOrDefaultAsync(new FilterStatusFromNameSpecification("Finalizado"));
            var response = new List<PendingActivitiesDto>();
            foreach (var opportunity in currentlyActiveOpportunities)
            {
                if (!response.Exists(x => x.FullName == opportunity.User!.FullName))
                {
                    var dto = new PendingActivitiesDto();
                    var numPending = opportunity.OpportunityActivities!.FindAll(x => x.StatusId == pendingStatus!.Id).Count();
                    var numPaused = opportunity.OpportunityActivities!.FindAll(x => x.StatusId == pausedStatus!.Id).Count();
                    var numFinished = opportunity.OpportunityActivities!.FindAll(x => x.StatusId == finishedStatus!.Id).Count();
                    dto.Id = opportunity.User!.Id;
                    dto.FullName = opportunity.User!.FullName;
                    dto.NumPendingActivities = numPending;
                    dto.NumFinishedActivities = numFinished;
                    dto.NumPausedActivities = numPaused;
                    response.Add(dto);
                }
                else
                {
                    var index = response.FindIndex(x => x.Id == opportunity.UserId);
                    var numPending = opportunity.OpportunityActivities!.FindAll(x => x.StatusId == pendingStatus!.Id).Count();
                    var numPaused = opportunity.OpportunityActivities!.FindAll(x => x.StatusId == pausedStatus!.Id).Count();
                    var numFinished = opportunity.OpportunityActivities!.FindAll(x => x.StatusId == finishedStatus!.Id).Count();
                    response[index].NumPendingActivities += numPending;
                    response[index].NumFinishedActivities += numFinished;
                    response[index].NumPausedActivities += numPaused;
                }

            }
            foreach (var dto in response)
            {
                dto.Total = dto.NumPendingActivities + dto.NumFinishedActivities + dto.NumPausedActivities;
                if (dto.Total > 0)
                {
                    dto.CompletionPercentage = (decimal)dto.NumFinishedActivities / dto.Total * 100;
                }
                else
                {
                    dto.CompletionPercentage = 0;
                }

            }
            return new Response<List<PendingActivitiesDto>>(response);
        }
    }
}
