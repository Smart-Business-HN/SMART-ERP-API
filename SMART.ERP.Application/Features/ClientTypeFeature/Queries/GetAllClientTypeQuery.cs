using AutoMapper;
using MediatR;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Application.DTOs.Customer;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.ClientTypeFeature.Queries
{
    public class GetAllClientTypeQuery : IRequest<Response<List<CustomerTypeDto>>>
    {
        public class GetAllClientTypeQueryHandler : IRequestHandler<GetAllClientTypeQuery, Response<List<CustomerTypeDto>>>
        {
            private readonly IRepositoryAsync<CustomerType> _repositoryAsync;
            private readonly IMapper _mapper;

            public GetAllClientTypeQueryHandler(IRepositoryAsync<CustomerType> repositoryAsync, IMapper mapper)
            {
                _repositoryAsync = repositoryAsync;
                _mapper = mapper;
            }

            public async Task<Response<List<CustomerTypeDto>>> Handle(GetAllClientTypeQuery request, CancellationToken cancellationToken)
            {
                var clientTypeList = await _repositoryAsync.ListAsync();
                var dto = _mapper.Map<List<CustomerTypeDto>>(clientTypeList);
                return new Response<List<CustomerTypeDto>>(dto);
            }
        }
    }
}
