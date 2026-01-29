using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Address;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.RegionSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.RegionFeature.Commands.UpdateRegionCommand
{
    public class UpdateRegionCommand : IRequest<Response<RegionDto>>
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int CountryId { get; set; }
        public bool IsActive { get; set; }
    }

    public class UpdateRegionCommandHandler : IRequestHandler<UpdateRegionCommand, Response<RegionDto>>
    {
        private readonly IRepositoryAsync<Region> _repositoryAsync;
        private readonly IRepositoryAsync<Country> _countryRepositoryAsync;
        private readonly IMapper _mapper;

        public UpdateRegionCommandHandler(IRepositoryAsync<Region> repositoryAsync, IRepositoryAsync<Country> countryRepositoryAsync, IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _countryRepositoryAsync = countryRepositoryAsync;
            _mapper = mapper;
        }

        public async Task<Response<RegionDto>> Handle(UpdateRegionCommand request, CancellationToken cancellationToken)
        {
            var checkRegion = await _repositoryAsync.GetByIdAsync(request.Id);
            if (checkRegion == null)
            {
                throw new KeyNotFoundException($"No se encontro la region con id {request.Id}");
            }
            var checkCountry = await _countryRepositoryAsync.GetByIdAsync(request.CountryId);
            if (checkCountry == null)
            {
                throw new KeyNotFoundException($"No se encontro el país con Id {request.CountryId}");
            }
            var checkRegionName = await _repositoryAsync.FirstOrDefaultAsync(new FilterRegionFromNameSpecification(request.Name, request.CountryId, request.Id));
            if (checkRegionName != null)
            {
                throw new ApiException($"Ya existe una region con el nombre {request.Name} para el pais {checkCountry.Name}");
            }
            checkRegion.Name = request.Name;
            checkRegion.CountryId = request.CountryId;
            checkRegion.IsActive = request.IsActive;
            await _repositoryAsync.UpdateAsync(checkRegion);
            await _repositoryAsync.SaveChangesAsync();
            var dto = _mapper.Map<RegionDto>(checkRegion);
            return new Response<RegionDto>(dto, $"Region {request.Name} actualizado exitosamente.");
        }
    }
}
