using MediatR;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.JwtService;
using SMART.ERP.Application.Specifications.OpportunitySpecification;
using SMART.ERP.Application.Specifications.UserSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Application.DTOs.Dashboard;

namespace SMART.ERP.Application.Features.DashboardFeature.Queries.OpportunityMetrics
{
    public class OpportunityFunnelStepMetricQuery : IRequest<Response<List<OpportunityStepMetricDto>>>
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int BranchOfficeId { get; set; }
    }

    public class OpportunityFunnelStepMetricQueryHandler : IRequestHandler<OpportunityFunnelStepMetricQuery, Response<List<OpportunityStepMetricDto>>>
    {
        private readonly IRepositoryAsync<Opportunity> _repositoryAsync;
        private readonly IRepositoryAsync<OpportunityStep> _stepRepositoryAsync;
        private readonly IJwtService _jwtService;
        private readonly IRepositoryAsync<User> _userRepositoryAsync;

        public OpportunityFunnelStepMetricQueryHandler(IRepositoryAsync<Opportunity> repositoryAsync, IRepositoryAsync<OpportunityStep> stepRepositoryAsync, IJwtService jwtService,
            IRepositoryAsync<User> userRepositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
            _stepRepositoryAsync = stepRepositoryAsync;
            _jwtService = jwtService;
            _userRepositoryAsync = userRepositoryAsync;
        }

        public async Task<Response<List<OpportunityStepMetricDto>>> Handle(OpportunityFunnelStepMetricQuery request, CancellationToken cancellationToken)
        {
            Guid uid = _jwtService.GetUidToken();
            var checkIfExist = await _userRepositoryAsync.FirstOrDefaultAsync(new UserIncludesSpecification(uid, null));
            if (checkIfExist == null)
            {
                throw new InvalidOperationException("Usuario Invalido");
            }
            var opportunities = new List<Opportunity>();
            if (checkIfExist.Role!.Name == "Manager")
            {
                opportunities = await _repositoryAsync.ListAsync(new FilterOpportunitiesinDatesSpecification(request.StartDate, request.EndDate, checkIfExist.BranchOfficeId));
            }
            else
            {
                opportunities = await _repositoryAsync.ListAsync(new FilterOpportunitiesinDatesSpecification(request.StartDate, request.EndDate, request.BranchOfficeId));
            }
            var steps = await _stepRepositoryAsync.ListAsync();
            var response = new List<OpportunityStepMetricDto>();
            foreach (var step in steps)
            {
                var dto = new OpportunityStepMetricDto();
                dto.Name = step.Name;
                var stepOpportunities = opportunities.FindAll(x => x.OpportunityStep!.Name == step.Name);
                if (step.Name == "Perdido" || step.Name == "Abandonado" || step.Name == "Ganado")
                {
                    if (request.StartDate.HasValue)
                    {
                        stepOpportunities = stepOpportunities.FindAll(x => x.ClosingDate.HasValue && x.ClosingDate.Value.Date >= request.StartDate.Value.Date);
                    }
                    if (request.EndDate.HasValue)
                    {
                        stepOpportunities = stepOpportunities.FindAll(x => x.ClosingDate.HasValue && x.ClosingDate.Value.Date <= request.EndDate.Value.Date);
                    }
                }
                dto.NumOpportunities = stepOpportunities.Count;
                dto.Total = stepOpportunities.Sum(a => a.Total);
                dto.NumQuotes = stepOpportunities.Sum(x => x.QuoteProducts.Sum(y => y.Quantity));
                response.Add(dto);

            }
            return new Response<List<OpportunityStepMetricDto>>(response);
        }
    }
}
