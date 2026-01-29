using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Customer;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.ClientCountrySpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.ClientCountryFeature.Queries
{
    public class GetAllClientCountryQuery : IRequest<Response<List<CountryDto>>>
    {
        public class GetAllClientCountryQueryHandler : IRequestHandler<GetAllClientCountryQuery, Response<List<CountryDto>>>
        {
            private readonly IRepositoryAsync<Country> _repositoryAsync;
            private readonly IMapper _mapper;

            public GetAllClientCountryQueryHandler(IRepositoryAsync<Country> repositoryAsync, IMapper mapper)
            {
                _repositoryAsync = repositoryAsync;
                _mapper = mapper;
            }

            public async Task<Response<List<CountryDto>>> Handle(GetAllClientCountryQuery request, CancellationToken cancellationToken)
            {
                var countryList = await _repositoryAsync.ListAsync(new ClientCountryIncludesSpecification());
                var response = _mapper.Map<List<CountryDto>>(countryList);
                return new Response<List<CountryDto>>(response);
            }
        }
    }
}
