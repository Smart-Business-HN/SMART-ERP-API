using AutoMapper;
using MediatR;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.MASTER.Domain.Entities;
using SMART.ERP.Application.DTOs.Customer;

namespace SMART.ERP.Application.Features.ClientHeadingFeature.Queries
{
    public class GetAllClientHeadingQuery : IRequest<Response<List<HeadingDto>>>
    {
        public class GetAllClientHeadingQueryHandler : IRequestHandler<GetAllClientHeadingQuery, Response<List<HeadingDto>>>
        {
            private readonly IRepositoryHNAsync<ClientHeading> _repositoryAsync;
            private readonly IMapper _mapper;

            public GetAllClientHeadingQueryHandler(IRepositoryHNAsync<ClientHeading> repositoryAsync, IMapper mapper)
            {
                _repositoryAsync = repositoryAsync;
                _mapper = mapper;
            }

            public async Task<Response<List<HeadingDto>>> Handle(GetAllClientHeadingQuery request, CancellationToken cancellationToken)
            {
                var headingList = await _repositoryAsync.ListAsync();
                var dto = _mapper.Map<List<HeadingDto>>(headingList);
                return new Response<List<HeadingDto>>(dto);
            }
        }
    }
}
