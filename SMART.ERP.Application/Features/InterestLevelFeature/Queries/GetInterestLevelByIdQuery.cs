using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Opportunity;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.InterestLevelFeature.Queries
{
    public class GetInterestLevelByIdQuery : IRequest<Response<InterestLevelDto>>
    {
        public int Id { get; set; }
    }

    public class GetInterestLevelByIdQueryHandler : IRequestHandler<GetInterestLevelByIdQuery, Response<InterestLevelDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<InterestLevel> _repositoryAsync;

        public GetInterestLevelByIdQueryHandler(IMapper mapper, IRepositoryAsync<InterestLevel> repositoryAsync)
        {
            _mapper = mapper;
            _repositoryAsync = repositoryAsync;
        }
        public async Task<Response<InterestLevelDto>> Handle(GetInterestLevelByIdQuery request, CancellationToken cancellationToken)
        {
            var interestLevel = await _repositoryAsync.GetByIdAsync(request.Id);
            var dto = _mapper.Map<InterestLevelDto>(interestLevel);
            return new Response<InterestLevelDto>(dto);
        }
    }
}
