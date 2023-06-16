using MediatR;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.ProspectSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Application.DTOs.Dashboard;

namespace SMART.ERP.Application.Features.DashboardFeature.Queries.ProspectMetrics
{
    public class ProspectStepsMetricsQuery : IRequest<Response<List<ProspectStepMetricDto>>>
    {
        public int? BranchOfficeId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public class ProspectStepsMetricQueryHandler : IRequestHandler<ProspectStepsMetricsQuery, Response<List<ProspectStepMetricDto>>>
    {
        private readonly IRepositoryAsync<Prospect> _repositoryAsync;
        private readonly IRepositoryAsync<ProspectStep> _stepRepositoryAsync;

        public ProspectStepsMetricQueryHandler(IRepositoryAsync<Prospect> repositoryAsync, IRepositoryAsync<ProspectStep> stepRepositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
            _stepRepositoryAsync = stepRepositoryAsync;
        }

        public async Task<Response<List<ProspectStepMetricDto>>> Handle(ProspectStepsMetricsQuery request, CancellationToken cancellationToken)
        {
            var steps = await _stepRepositoryAsync.ListAsync();
            var response = new List<ProspectStepMetricDto>();
            var prospects = new List<Prospect>();
            if (request.BranchOfficeId != null && request.BranchOfficeId != 0)
            {
                prospects = await _repositoryAsync.ListAsync(new FilterProspectByBranchSpecification((int)request.BranchOfficeId));
            }
            else
            {
                prospects = await _repositoryAsync.ListAsync(new ProspectIncludesSpecification(null));
            }
            if (request.StartDate != null && request.EndDate != null)
            {
                var closed = prospects.FindAll(x => (x.ProspectStep!.Name == "Convertido" || x.ProspectStep.Name == "No Calificado")
                    && x.ModificationDate.HasValue && x.ModificationDate.Value.Date >= request.StartDate.Value.Date
                    && x.ModificationDate.Value.Date <= request.EndDate.Value.Date);
                var notClosed = prospects.FindAll(x => x.ProspectStep!.Name != "Convertido" && x.ProspectStep.Name != "No Calificado"
                    && x.CreationDate.Date >= request.StartDate.Value.Date && x.CreationDate.Date <= request.EndDate.Value.Date);
                prospects = new HashSet<Prospect>(closed.Concat(notClosed)).ToList();
            }
            foreach (var step in steps)
            {
                ProspectStepMetricDto dto = new();
                var prospectsInStep = prospects.FindAll(x => x.ProspectStepId == step.Id);
                dto.Name = step.Name;
                dto.NumProspects = prospectsInStep.Count;
                response.Add(dto);
            }
            return new Response<List<ProspectStepMetricDto>>(response);
        }
    }
}
