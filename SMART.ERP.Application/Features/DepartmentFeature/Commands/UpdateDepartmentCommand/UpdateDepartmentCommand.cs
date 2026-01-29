using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Address;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.DepartmentFeature.Commands.UpdateDepartmentCommand
{
    public class UpdateDepartmentCommand : IRequest<Response<DepartmentDto>>
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int CountryId { get; set; }
        public bool IsActive { get; set; }
    }

    public class UpdateDepartmentCommandHandler : IRequestHandler<UpdateDepartmentCommand, Response<DepartmentDto>>
    {
        private readonly IRepositoryAsync<Department> _repositoryAsync;
        private readonly IRepositoryAsync<Country> _countryRepositoryAsync;
        private readonly IMapper _mapper;

        public UpdateDepartmentCommandHandler(IRepositoryAsync<Department> repositoryAsync, IRepositoryAsync<Country> countryRepositoryAsync,
            IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _countryRepositoryAsync = countryRepositoryAsync;
            _mapper = mapper;
        }
        public async Task<Response<DepartmentDto>> Handle(UpdateDepartmentCommand request, CancellationToken cancellationToken)
        {
            var checkDepartment = await _repositoryAsync.GetByIdAsync(request.Id);
            if (checkDepartment == null)
            {
                throw new KeyNotFoundException($"No se encontro el departamento con id {request.Id}");
            }
            var checkRegion = await _countryRepositoryAsync.GetByIdAsync(request.CountryId);
            if (checkRegion == null)
            {
                throw new KeyNotFoundException($"No se encontro el país con id {request.CountryId}");
            }

            checkDepartment.Name = request.Name;
            checkDepartment.IsActive = request.IsActive;
            checkDepartment.RegionId = request.CountryId;

            await _repositoryAsync.UpdateAsync(checkDepartment);
            await _repositoryAsync.SaveChangesAsync();

            var dto = _mapper.Map<DepartmentDto>(checkDepartment);

            return new Response<DepartmentDto>(dto, "Actualizado correctamente");
        }
    }
}
