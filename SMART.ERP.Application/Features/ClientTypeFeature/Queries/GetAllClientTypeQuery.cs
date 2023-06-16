using AutoMapper;
using MediatR;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.MASTER.Domain.Entities;
using SMART.ERP.Application.DTOs.Customer;

namespace SMART.ERP.Application.Features.ClientTypeFeature.Queries
{
    public class GetAllClientTypeQuery : IRequest<Response<List<ClientTypeDto>>>
    {
        public class GetAllClientTypeQueryHandler : IRequestHandler<GetAllClientTypeQuery, Response<List<ClientTypeDto>>>
        {
            private readonly IRepositoryHNAsync<ClientType> _repositoryAsync;
            private readonly IMapper _mapper;

            public GetAllClientTypeQueryHandler(IRepositoryHNAsync<ClientType> repositoryAsync, IMapper mapper)
            {
                _repositoryAsync = repositoryAsync;
                _mapper = mapper;
            }

            public async Task<Response<List<ClientTypeDto>>> Handle(GetAllClientTypeQuery request, CancellationToken cancellationToken)
            {
                var clientTypeList = await _repositoryAsync.ListAsync();
                var dto = _mapper.Map<List<ClientTypeDto>>(clientTypeList);
                return new Response<List<ClientTypeDto>>(dto);
            }
        }
    }
}
