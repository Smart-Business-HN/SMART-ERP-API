using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.AdvisorDepartment;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.AdvisorDepartmentSpecification;
using SMART.ERP.Application.Specifications.UserSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.AdvisorDepartmentFeature.Commands.CreateAdvisorDepartmentCommand
{
    public class CreateAdvisorDepartmentCommand : IRequest<Response<List<AdvisorDepartmentDto>>>
    {
        public Guid UserId { get; set; }
        public List<int> DepartmentId { get; set; } = null!;
    }

    public class CreateAdvisorDepartmentCommandHandler : IRequestHandler<CreateAdvisorDepartmentCommand, Response<List<AdvisorDepartmentDto>>>
    {
        private readonly IRepositoryAsync<AdvisorDepartment> _repositoryAsync;
        private readonly IRepositoryAsync<User> _userRepositoryAsync;
        private readonly IRepositoryAsync<Department> _departmentRepositoryAsync;
        private readonly IMapper _mapper;

        public CreateAdvisorDepartmentCommandHandler(IRepositoryAsync<AdvisorDepartment> repositoryAsync, IRepositoryAsync<User> userRepositoryAsync,
            IRepositoryAsync<Department> departmentRepositoryAsync, IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _userRepositoryAsync = userRepositoryAsync;
            _departmentRepositoryAsync = departmentRepositoryAsync;
            _mapper = mapper;
        }
        public async Task<Response<List<AdvisorDepartmentDto>>> Handle(CreateAdvisorDepartmentCommand request, CancellationToken cancellationToken)
        {
            var checkUser = await _userRepositoryAsync.FirstOrDefaultAsync(new UserIncludesSpecification(request.UserId, null));
            if (checkUser == null)
            {
                throw new KeyNotFoundException($"No se encontro el usuario con id {request.UserId}");
            }
            if (checkUser.Role!.Selector != "SalesAdvisor")
            {
                throw new ApiException("solo los asesores de venta pueden tender departamentos asignados");
            }
            foreach (var id in request.DepartmentId)
            {
                var checkDepartment = await _departmentRepositoryAsync.GetByIdAsync(id);
                if (checkDepartment == null)
                {
                    throw new KeyNotFoundException($"No se encontro el departamento con id {id}");
                }
                var checkAssignment = await _repositoryAsync.FirstOrDefaultAsync(new FilterAdvisorDepartmentAssignment(id, request.UserId));
                if (checkAssignment != null)
                {
                    throw new ApiException($"Este usuario ya tiene asignado el departamento {checkDepartment.Name}");
                }
            }
            var response = new List<AdvisorDepartmentDto>();
            foreach (var id in request.DepartmentId)
            {
                var newRecord = new AdvisorDepartment();
                newRecord.UserId = request.UserId;
                newRecord.DepartmentId = id;
                var newAdvisorDepartment = await _repositoryAsync.AddAsync(newRecord);
                await _repositoryAsync.SaveChangesAsync();
                var dto = _mapper.Map<AdvisorDepartmentDto>(newAdvisorDepartment);
                response.Add(dto);
            }
            return new Response<List<AdvisorDepartmentDto>>(response, $"Asignado correctamente");
        }
    }
}
