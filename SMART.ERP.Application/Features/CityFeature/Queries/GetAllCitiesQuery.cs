using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Address;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.CitySpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.CityFeature.Queries
{
    public class GetAllCitiesQuery : IRequest<PagedResponse<List<CityDto>>>
    {
        public string? Parameter { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? Order { get; set; }
        public string? Column { get; set; }
        public bool All { get; set; }

        public class GetAllCitiesQueryHandler : IRequestHandler<GetAllCitiesQuery, PagedResponse<List<CityDto>>>
        {
            private readonly IMapper _mapper;
            private readonly IRepositoryAsync<City> _repositoryAsync;
            public GetAllCitiesQueryHandler(IMapper mapper, IRepositoryAsync<City> repositoryAsync)
            {
                _mapper = mapper;
                _repositoryAsync = repositoryAsync;
            }
            public async Task<PagedResponse<List<CityDto>>> Handle(GetAllCitiesQuery request, CancellationToken cancellationToken)
            {
                if (request.All)
                {
                    request.PageNumber = 0;
                    request.PageSize = await _repositoryAsync.CountAsync();
                }
                var cities = await _repositoryAsync.ListAsync(new FilterAndPaginationCitySpecification(request.Parameter, request.PageNumber, request.PageSize, request.Order, request.Column));
                var dto = _mapper.Map<List<CityDto>>(cities);
                return new PagedResponse<List<CityDto>>(dto, request.PageNumber, request.PageSize, request.All ? request.PageSize : await _repositoryAsync.CountAsync());
            }
        }
    }
}
