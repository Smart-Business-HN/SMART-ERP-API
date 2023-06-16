using AutoMapper;
using MediatR;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.DepartmentSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Application.DTOs.Address;

namespace SMART.ERP.Application.Features.RegionFeature.Commands.AssignRegionDepartmentCommand
{
    public class AssignRegionDepartmentCommand : IRequest<Response<RegionDto>>
    {
        public int Id { get; set; }
        public List<int> Departments { get; set; } = null!;
    }

    public class AssignRegionDepartmentCommandHandler : IRequestHandler<AssignRegionDepartmentCommand, Response<RegionDto>>
    {
        private readonly IRepositoryAsync<Region> _repositoryAsync;
        private readonly IRepositoryAsync<Department> _departmentRepositoryAsync;
        private readonly IMapper _mapper;

        public AssignRegionDepartmentCommandHandler(IRepositoryAsync<Region> repositoryAsync, IRepositoryAsync<Department> departmentRepositoryAsync,
            IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _departmentRepositoryAsync = departmentRepositoryAsync;
            _mapper = mapper;
        }

        public async Task<Response<RegionDto>> Handle(AssignRegionDepartmentCommand request, CancellationToken cancellationToken)
        {
            var checkRegion = await _repositoryAsync.GetByIdAsync(request.Id);
            if (checkRegion == null)
            {
                throw new KeyNotFoundException($"No se encontro la region con Id {request.Id}");
            }
            var departments = await _departmentRepositoryAsync.ListAsync(new FilterListDepartmentSpecification(request.Departments));
            if (departments.Count < 1)
            {
                throw new KeyNotFoundException($"No se encontro ningun departamento con los IDs proveidos.");
            }

            foreach (var department in departments)
            {
                department.RegionId = checkRegion.Id;
            }

            await _departmentRepositoryAsync.UpdateRangeAsync(departments);
            await _departmentRepositoryAsync.SaveChangesAsync();

            var dto = _mapper.Map<RegionDto>(checkRegion);
            dto.Departments = _mapper.Map<List<DepartmentDto>>(departments);

            return new Response<RegionDto>(dto, "Departamentos Asignados correctamente");
        }
    }
}
