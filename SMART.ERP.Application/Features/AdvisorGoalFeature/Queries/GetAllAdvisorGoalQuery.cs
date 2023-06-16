using AutoMapper;
using MediatR;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.AdvisorGoalSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Application.DTOs.AdvisorGoal;

namespace SMART.ERP.Application.Features.AdvisorGoalFeature.Queries
{
    public class GetAllAdvisorGoalQuery : IRequest<Response<List<AdvisorGoalDto>>>
    {
        public DateTime Date { get; set; }
    }

    public class GetAllAdvisorGoalQueryHandler : IRequestHandler<GetAllAdvisorGoalQuery, Response<List<AdvisorGoalDto>>>
    {
        private readonly IRepositoryAsync<AdvisorGoal> _repositoryAsync;
        private readonly IMapper _mapper;

        public GetAllAdvisorGoalQueryHandler(IRepositoryAsync<AdvisorGoal> repositoryAsync, IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _mapper = mapper;
        }
        public async Task<Response<List<AdvisorGoalDto>>> Handle(GetAllAdvisorGoalQuery request, CancellationToken cancellationToken)
        {
            var advisorGoals = await _repositoryAsync.ListAsync(new FilterAdvisorGoalByMonthSpecification(null, null, request.Date));
            var dto = _mapper.Map<List<AdvisorGoalDto>>(advisorGoals);
            return new Response<List<AdvisorGoalDto>>(dto);
        }
    }
}
