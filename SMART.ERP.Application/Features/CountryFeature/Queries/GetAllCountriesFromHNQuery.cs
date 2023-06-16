using AutoMapper;
using MediatR;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.CountrySpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.MASTER.Domain.Entities;
using SMART.ERP.Application.DTOs.Address;

namespace SMART.ERP.Application.Features.CountryFeature.Queries
{
    public class GetAllCountriesFromHNQuery : IRequest<PagedResponse<List<ClientCountryDto>>>
    {
        public string? Parameter { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? Order { get; set; }
        public string? Column { get; set; }
        public bool All { get; set; }
    }

    public class GetAllCountriesFromHNQueryHandler : IRequestHandler<GetAllCountriesFromHNQuery, PagedResponse<List<ClientCountryDto>>>
    {
        private readonly IRepositoryHNAsync<ClientCountry> _repositoryAsync;
        private readonly IMapper _mapper;

        public GetAllCountriesFromHNQueryHandler(IRepositoryHNAsync<ClientCountry> repositoryAsync, IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _mapper = mapper;
        }

        public async Task<PagedResponse<List<ClientCountryDto>>> Handle(GetAllCountriesFromHNQuery request, CancellationToken cancellationToken)
        {
            var countries = await _repositoryAsync.ListAsync(new PagedCountryFromHNSpecification(
                request.Parameter, request.PageNumber, request.PageSize, request.Order, request.Column));
            var dto = _mapper.Map<List<ClientCountryDto>>(countries);
            return new PagedResponse<List<ClientCountryDto>>(dto, request.PageNumber, request.PageSize, countries.Count);
        }
    }
}
