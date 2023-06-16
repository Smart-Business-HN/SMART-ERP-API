using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Opportunity;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.InterestLevelSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.InterestLevelFeature.Queries
{
    public class GetAllInterestLevelsQuery : IRequest<PagedResponse<List<InterestLevelDto>>>
    {
        public string? Parameter { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? Order { get; set; }
        public string? Column { get; set; }
        public bool All { get; set; }
        public class GetAllInterestLevelsQueryHandle : IRequestHandler<GetAllInterestLevelsQuery, PagedResponse<List<InterestLevelDto>>>
        {
            private readonly IMapper _mapper;
            private readonly IRepositoryAsync<InterestLevel> _repositoryAsync;

            public GetAllInterestLevelsQueryHandle(IMapper mapper, IRepositoryAsync<InterestLevel> repositoryAsync)
            {
                _mapper = mapper;
                _repositoryAsync = repositoryAsync;
            }

            public async Task<PagedResponse<List<InterestLevelDto>>> Handle(GetAllInterestLevelsQuery request, CancellationToken cancellationToken)
            {
                if (request.All)
                {
                    request.PageNumber = 0;
                    request.PageSize = await _repositoryAsync.CountAsync();
                }

                var interestLevels = await _repositoryAsync.ListAsync(
                    new FilterAndPaginationInterestLevelSpecification(request.Parameter, request.PageNumber, request.PageSize, request.Order, request.Column));
                var dto = _mapper.Map<List<InterestLevelDto>>(interestLevels);
                return new PagedResponse<List<InterestLevelDto>>(dto, request.PageNumber, request.PageSize, request.All ? request.PageSize : await _repositoryAsync.CountAsync());
            }
        }
    }
}
