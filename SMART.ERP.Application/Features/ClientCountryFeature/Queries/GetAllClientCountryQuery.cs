using AutoMapper;
using MediatR;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.ClientCountrySpecification;
using SMART.ERP.Application.Wrappers;
using SMART.MASTER.Domain.Entities;
using SMART.ERP.Application.DTOs.Address;

namespace SMART.ERP.Application.Features.ClientCountryFeature.Queries
{
    public class GetAllClientCountryQuery : IRequest<Response<List<ClientCountryDto>>>
    {
        public class GetAllClientCountryQueryHandler : IRequestHandler<GetAllClientCountryQuery, Response<List<ClientCountryDto>>>
        {
            private readonly IRepositoryHNAsync<ClientCountry> _repositoryAsync;
            private readonly IMapper _mapper;

            public GetAllClientCountryQueryHandler(IRepositoryHNAsync<ClientCountry> repositoryAsync, IMapper mapper)
            {
                _repositoryAsync = repositoryAsync;
                _mapper = mapper;
            }

            public async Task<Response<List<ClientCountryDto>>> Handle(GetAllClientCountryQuery request, CancellationToken cancellationToken)
            {
                var countryList = await _repositoryAsync.ListAsync(new ClientCountryIncludesSpecification());
                var response = _mapper.Map<List<ClientCountryDto>>(countryList);
                return new Response<List<ClientCountryDto>>(response);
            }
        }
    }
}
