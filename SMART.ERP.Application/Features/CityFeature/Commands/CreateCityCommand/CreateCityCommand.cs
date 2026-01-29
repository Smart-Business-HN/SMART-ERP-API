using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.OutputCaching;
using SMART.ERP.Application.DTOs.Address;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.CitySpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.CityFeature.Commands.CreateCityCommand
{
    public class CreateCityCommand : IRequest<Response<CityDto>>
    {
        public string Name { get; set; } = null!;
        public int DepartmentId { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateCityCommandHandler : IRequestHandler<CreateCityCommand, Response<CityDto>>
    {
        private readonly IRepositoryAsync<City> _repositoryAsync;
        private readonly IRepositoryAsync<Department> _departmentRepositoryAsync;
        private readonly IMapper _mapper;
        private readonly IOutputCacheStore _outputCacheStored;

        public CreateCityCommandHandler(IRepositoryAsync<City> repositoryAsync, IRepositoryAsync<Department> departmentRepositoryAsync,
            IMapper mapper, IOutputCacheStore outputCacheStored)
        {
            _repositoryAsync = repositoryAsync;
            _departmentRepositoryAsync = departmentRepositoryAsync;
            _mapper = mapper;
            _outputCacheStored = outputCacheStored;
        }

        public async Task<Response<CityDto>> Handle(CreateCityCommand request, CancellationToken cancellationToken)
        {
            var checkbyName = await _repositoryAsync.FirstOrDefaultAsync(new FilterCityFromNameSpecification(request.Name, null));
            if (checkbyName != null)
            {
                throw new ApiException($"Ya existe una ciudad con el nombre {request.Name}");
            }

            var checkDepartment = await _departmentRepositoryAsync.GetByIdAsync(request.DepartmentId);
            if (checkDepartment == null)
            {
                throw new KeyNotFoundException($"No se encontro el departamento con id {request.DepartmentId}");
            }

            var newRecord = _mapper.Map<City>(request);
            var response = await _repositoryAsync.AddAsync(newRecord);
            await _repositoryAsync.SaveChangesAsync();
            await _outputCacheStored.EvictByTagAsync("cache_cities", cancellationToken);
            var dto = _mapper.Map<CityDto>(response);
            return new Response<CityDto>(dto, $"{request.Name} agregado correctamente");
        }
    }
}
