using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Opportunity;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.TypeActivitySpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.TypeActivityFeature.Queries
{
    public class GetAllTypeActivitiesQuery : IRequest<PagedResponse<List<TypeActivityDto>>>
    {
        public string? Parameter { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? Order { get; set; }
        public string? Column { get; set; }
        public bool All { get; set; }
        public class GetAllTypeActivitiesQueryHandler : IRequestHandler<GetAllTypeActivitiesQuery, PagedResponse<List<TypeActivityDto>>>
        {
            private readonly IMapper _mapper;
            private readonly IRepositoryAsync<TypeActivity> _repositoryAsync;

            public GetAllTypeActivitiesQueryHandler(IMapper mapper, IRepositoryAsync<TypeActivity> repositoryAsync)
            {
                _mapper = mapper;
                _repositoryAsync = repositoryAsync;
            }
            public async Task<PagedResponse<List<TypeActivityDto>>> Handle(GetAllTypeActivitiesQuery request, CancellationToken cancellationToken)
            {
                if (request.All)
                {
                    request.PageNumber = 0;
                    request.PageSize = await _repositoryAsync.CountAsync();
                }

                var typeActivities = await _repositoryAsync.ListAsync(new PaginationTypeActivitySpecification(request.Parameter, request.PageNumber,
                    request.PageSize, request.Order, request.Column));
                var dto = _mapper.Map<List<TypeActivityDto>>(typeActivities);
                return new PagedResponse<List<TypeActivityDto>>(dto, request.PageNumber, request.PageSize, request.All ? request.PageSize : await _repositoryAsync.CountAsync());
            }
        }
    }
}
