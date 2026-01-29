using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Address;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.RegionSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.RegionFeature.Commands.CreateRegionCommand
{
    public class CreateRegionCommand : IRequest<Response<RegionDto>>
    {
        public string Name { get; set; } = null!;
        public int CountryId { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateRegionCommandHandler : IRequestHandler<CreateRegionCommand, Response<RegionDto>>
    {
        private readonly IRepositoryAsync<Region> _repositoryAsync;
        private readonly IRepositoryAsync<Country> _countryRepositoryAsync;
        private readonly IMapper _mapper;

        public CreateRegionCommandHandler(IRepositoryAsync<Region> repositoryAsync, IRepositoryAsync<Country> countryRepositoryAsync, IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _countryRepositoryAsync = countryRepositoryAsync;
            _mapper = mapper;
        }

        public async Task<Response<RegionDto>> Handle(CreateRegionCommand request, CancellationToken cancellationToken)
        {
            var checkCountry = await _countryRepositoryAsync.GetByIdAsync(request.CountryId);
            if (checkCountry == null)
            {
                throw new KeyNotFoundException($"No se encontro el país con Id {request.CountryId}");
            }
            var checkRegion = await _repositoryAsync.FirstOrDefaultAsync(new FilterRegionFromNameSpecification(request.Name, request.CountryId, null));
            if (checkRegion != null)
            {
                throw new ApiException($"Ya existe una region con el nombre {request.Name} para el pais {checkCountry.Name}");
            }
            var newRecord = _mapper.Map<Region>(request);
            var response = await _repositoryAsync.AddAsync(newRecord);
            await _repositoryAsync.SaveChangesAsync();
            var dto = _mapper.Map<RegionDto>(response);
            return new Response<RegionDto>(dto, $"Region {request.Name} creada exitosamente.");
        }
    }
}
