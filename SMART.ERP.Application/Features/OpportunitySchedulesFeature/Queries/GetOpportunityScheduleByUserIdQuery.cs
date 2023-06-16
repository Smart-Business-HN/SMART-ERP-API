using MediatR;
using SMART.ERP.Application.DTOs.Opportunity;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.JwtService;
using SMART.ERP.Application.Specifications.OpportunityScheduleSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.OpportunitySchedulesFeature.Queries
{
    public class GetOpportunityScheduleByUserIdQuery : IRequest<Response<OpportunityScheduleDto>>
    {
        public class GetOpportunityScheduleByUserIdQueryHandler : IRequestHandler<GetOpportunityScheduleByUserIdQuery, Response<OpportunityScheduleDto>>
        {
            private readonly IRepositoryAsync<OpportunitySchedules> _repositoryAsync;
            private readonly IJwtService _jwtService;

            public GetOpportunityScheduleByUserIdQueryHandler(IRepositoryAsync<OpportunitySchedules> repositoryAsync, IJwtService jwtService)
            {
                _repositoryAsync = repositoryAsync;
                _jwtService = jwtService;
            }

            public async Task<Response<OpportunityScheduleDto>> Handle(GetOpportunityScheduleByUserIdQuery request, CancellationToken cancellationToken)
            {
                Guid guid = _jwtService.GetUidToken();
                var schedule = await _repositoryAsync.FirstOrDefaultAsync(new FilterOpportunityScheduleByUserSpecification(guid));
                var response = new OpportunityScheduleDto();
                if (schedule != null)
                {
                    response.Schedule = schedule.OpportunityAge;
                }
                return new Response<OpportunityScheduleDto>(response);
            }
        }
    }
}
