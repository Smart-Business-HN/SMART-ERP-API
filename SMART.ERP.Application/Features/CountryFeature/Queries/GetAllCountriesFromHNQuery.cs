using AutoMapper;
using MediatR;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.CountrySpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Application.DTOs.Customer;

namespace SMART.ERP.Application.Features.CountryFeature.Queries
{
    public class GetAllCountriesFromHNQuery : IRequest<PagedResponse<List<CountryDto>>>
    {
        public string? Parameter { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? Order { get; set; }
        public string? Column { get; set; }
        public bool All { get; set; }
    }

    public class GetAllCountriesFromHNQueryHandler : IRequestHandler<GetAllCountriesFromHNQuery, PagedResponse<List<CountryDto>>>
    {
        private readonly IRepositoryAsync<Country> _repositoryAsync;
        private readonly IMapper _mapper;

        public GetAllCountriesFromHNQueryHandler(IRepositoryAsync<Country> repositoryAsync, IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _mapper = mapper;
        }

        public async Task<PagedResponse<List<CountryDto>>> Handle(GetAllCountriesFromHNQuery request, CancellationToken cancellationToken)
        {
            var countries = await _repositoryAsync.ListAsync(new PagedCountryFromHNSpecification(
                request.Parameter, request.PageNumber, request.PageSize, request.Order, request.Column));
            var dto = _mapper.Map<List<CountryDto>>(countries);
            return new PagedResponse<List<CountryDto>>(dto, request.PageNumber, request.PageSize, countries.Count);
        }
    }
}
