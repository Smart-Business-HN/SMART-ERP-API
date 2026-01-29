using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Address;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.CityFeature.Queries
{
    public class GetCityByIdQuery : IRequest<Response<CityDto>>
    {
        public int Id { get; set; }
    }
    public class GetCityByIdQueryHandler : IRequestHandler<GetCityByIdQuery, Response<CityDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<City> _repositoryAsync;

        public GetCityByIdQueryHandler(IMapper mapper, IRepositoryAsync<City> repositoryAsync)
        {
            _mapper = mapper;
            _repositoryAsync = repositoryAsync;
        }

        public async Task<Response<CityDto>> Handle(GetCityByIdQuery request, CancellationToken cancellationToken)
        {
            var city = await _repositoryAsync.GetByIdAsync(request.Id);
            if (city == null)
            {
                throw new KeyNotFoundException($"Registro no encontrado con el id {request.Id}");
            }
            var dto = _mapper.Map<CityDto>(city);
            return new Response<CityDto>(dto);
        }
    }
}
