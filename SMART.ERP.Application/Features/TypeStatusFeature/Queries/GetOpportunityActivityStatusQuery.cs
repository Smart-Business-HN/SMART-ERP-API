using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Status;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.TypeStatusSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.TypeStatusFeature.Queries
{
    public class GetOpportunityActivityStatusQuery : IRequest<Response<TypeStatusDto>>
    {
        public class GetOpportunityActivityStatusHandler : IRequestHandler<GetOpportunityActivityStatusQuery, Response<TypeStatusDto>>
        {
            private readonly IRepositoryAsync<TypeStatus> _repositoryAsync;
            private readonly IMapper _mapper;

            public GetOpportunityActivityStatusHandler(IRepositoryAsync<TypeStatus> repositoryAsync, IMapper mapper)
            {
                _repositoryAsync = repositoryAsync;
                _mapper = mapper;
            }

            public async Task<Response<TypeStatusDto>> Handle(GetOpportunityActivityStatusQuery request, CancellationToken cancellationToken)
            {
                var typeStatusList = await _repositoryAsync.FirstOrDefaultAsync(new FilterOpportunityActivityStatusSpecification());
                var dto = _mapper.Map<TypeStatusDto>(typeStatusList);
                return new Response<TypeStatusDto>(dto);
            }
        }
    }
}
