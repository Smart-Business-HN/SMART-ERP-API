using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.OutputCaching;
using SMART.ERP.Application.DTOs.Address;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.CityFeature.Commands.UpdateCityCommand
{
    public class UpdateCityCommand : IRequest<Response<CityDto>>
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int DepartmentId { get; set; }
        public bool IsActive { get; set; }
    }

    public class UpdateCityCommandHandler : IRequestHandler<UpdateCityCommand, Response<CityDto>>
    {
        private readonly IRepositoryAsync<City> _repositoryAsync;
        private readonly IRepositoryAsync<Department> _departmentRepositoryAsync;
        private readonly IMapper _mapper;
        private readonly IOutputCacheStore _outputCacheStored;

        public UpdateCityCommandHandler(IRepositoryAsync<City> repositoryAsync, IRepositoryAsync<Department> departmentRepositoryAsync,
            IMapper mapper, IOutputCacheStore outputCacheStored)
        {
            _repositoryAsync = repositoryAsync;
            _departmentRepositoryAsync = departmentRepositoryAsync;
            _mapper = mapper;
            _outputCacheStored = outputCacheStored;
        }

        public async Task<Response<CityDto>> Handle(UpdateCityCommand request, CancellationToken cancellationToken)
        {
            var checkCity = await _repositoryAsync.GetByIdAsync(request.Id);
            if (checkCity == null)
            {
                throw new KeyNotFoundException($"No se encontro la ciudad con id {request.Id}");
            }

            var checkDepartment = await _departmentRepositoryAsync.GetByIdAsync(request.DepartmentId);
            if (checkDepartment == null)
            {
                throw new KeyNotFoundException($"No se encontro el departamento con id {request.DepartmentId}");
            }

            checkCity.Name = request.Name;
            checkCity.DepartmentId = request.DepartmentId;
            checkCity.IsActive = request.IsActive;

            await _repositoryAsync.UpdateAsync(checkCity);
            await _repositoryAsync.SaveChangesAsync();
            await _outputCacheStored.EvictByTagAsync("cache_cities", cancellationToken);
            var dto = _mapper.Map<CityDto>(checkCity);
            return new Response<CityDto>(dto, $"{request.Name} actualizado correctamente");
        }
    }
}
