using AutoMapper;
using MediatR;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.AdvisorGoalSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.AdvisorGoalFeature.Queries
{
    public class CheckAdvisorGoalQuery : IRequest<Response<List<decimal>>>
    {
        public Guid UserId { get; set; }
    }

    public class CheckAdvisorGoalHandler : IRequestHandler<CheckAdvisorGoalQuery, Response<List<decimal>>>
    {
        private readonly IRepositoryAsync<AdvisorGoal> _repositoryAsync;
        private readonly IMapper _mapper;

        public CheckAdvisorGoalHandler(IRepositoryAsync<AdvisorGoal> repositoryAsync, IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _mapper = mapper;
        }
        public async Task<Response<List<decimal>>> Handle(CheckAdvisorGoalQuery request, CancellationToken cancellationToken)
        {
            var checkAdvisorGoal = await _repositoryAsync.ListAsync(new FilterAdvisorGoalByYearSpecification(DateTime.Now.Year, request.UserId));
            var response = new List<decimal> { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            foreach (var goal in checkAdvisorGoal)
            {
                response[goal.InitDate.Month - 1] = goal.Goal;
            }
            return new Response<List<decimal>>(response);
        }
    }
}
