using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Customer;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.CountrySpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.CountryFeature.Queries
{
    public class GetAllCountriesQuery : IRequest<PagedResponse<List<CountryDto>>>
    {
        public string? Parameter { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? Order { get; set; }
        public string? Column { get; set; }
        public bool All { get; set; }
        public bool IncludeCities { get; set; } = true;
    }

    public class GetAllCountriesQueryHandler : IRequestHandler<GetAllCountriesQuery, PagedResponse<List<CountryDto>>>
    {
        private readonly IRepositoryAsync<Country> _repositoryAsync;
        private readonly IMapper _mapper;

        public GetAllCountriesQueryHandler(IRepositoryAsync<Country> repositoryAsync, IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _mapper = mapper;
        }

        public async Task<PagedResponse<List<CountryDto>>> Handle(GetAllCountriesQuery request, CancellationToken cancellationToken)
        {
            var countries = await _repositoryAsync.ListAsync(new PagedCountrySpecification(
                request.Parameter, request.PageNumber, request.PageSize, request.Order, request.Column, request.IncludeCities));
            var dto = _mapper.Map<List<CountryDto>>(countries);
            return new PagedResponse<List<CountryDto>>(dto, request.PageNumber, request.PageSize, countries.Count);
        }
    }
}
