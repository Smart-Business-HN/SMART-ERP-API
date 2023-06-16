using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Opportunity;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.OpportunityActivitySpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.OpportunityActivityFeature.Queries
{
    public class GetAllOpportunityActivitiesQuery : IRequest<PagedResponse<List<OpportunityActivityDto>>>
    {
        public string? Parameter { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? Order { get; set; }
        public string? Column { get; set; }
        public bool All { get; set; }

        public class GetAllOpportunityActivitiesQueryHandler : IRequestHandler<GetAllOpportunityActivitiesQuery, PagedResponse<List<OpportunityActivityDto>>>
        {
            private readonly IMapper _mapper;
            private readonly IRepositoryAsync<OpportunityActivity> _repositoryAsync;

            public GetAllOpportunityActivitiesQueryHandler(IMapper mapper, IRepositoryAsync<OpportunityActivity> repositoryAsync)
            {
                _mapper = mapper;
                _repositoryAsync = repositoryAsync;
            }
            public async Task<PagedResponse<List<OpportunityActivityDto>>> Handle(GetAllOpportunityActivitiesQuery request, CancellationToken cancellationToken)
            {
                if (request.All)
                {
                    request.PageNumber = 0;
                    request.PageSize = await _repositoryAsync.CountAsync();
                }

                var opportunityActivities = await _repositoryAsync.ListAsync(
                    new PaginationOpportunityActivitySpecification(request.Parameter, request.PageNumber, request.PageSize, request.Order, request.Column));
                var dto = _mapper.Map<List<OpportunityActivityDto>>(opportunityActivities);
                return new PagedResponse<List<OpportunityActivityDto>>(dto, request.PageNumber, request.PageSize, request.All ? request.PageSize : await _repositoryAsync.CountAsync());
            }
        }
    }
}
