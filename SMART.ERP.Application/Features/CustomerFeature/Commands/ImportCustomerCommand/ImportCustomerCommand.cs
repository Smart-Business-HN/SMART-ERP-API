using AutoMapper;
using MediatR;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.JwtService;
using SMART.ERP.Application.Specifications.CustomerSpecification;
using SMART.ERP.Application.Specifications.DepartmentSpecification;
using SMART.ERP.Application.Specifications.UserSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Application.DTOs.Customer;
using SMART.ERP.Application.Services.AssignUserToOpportunityService;
using SMART.ERP.Application.Services.RegisterClientService;

namespace SMART.ERP.Application.Features.CustomerFeature.Commands.ImportCustomerCommand
{
    public class ImportCustomerCommand : IRequest<Response<CustomerDto>>
    {
        public Guid Id { get; set; }
    }

    public class ImportCustomerCommandHandler : IRequestHandler<ImportCustomerCommand, Response<CustomerDto>>
    {
        private readonly IRepositoryAsync<Customer> _repositoryHNAsync;
        private readonly IRepositoryAsync<Customer> _repositoryAsync;
        private readonly IMapper _mapper;
        private readonly IRegisterClientService _registerClientService;
        private readonly IRepositoryAsync<Department> _departmentRepositoryAsync;
        private readonly IAssignUserToOpportunityService _assignUserToOpportunityService;
        private readonly IRepositoryAsync<Department> _clientDepartmentRepositoryAsync;
        private readonly IJwtService _jwtService;
        private readonly IRepositoryAsync<User> _userRepositoryAsync;

        public ImportCustomerCommandHandler(IRepositoryAsync<Customer> repositoryHNAsync, IRepositoryAsync<Customer> repositoryAsync,
            IMapper mapper, IRegisterClientService registerClientService, IRepositoryAsync<Department> departmentRepositoryAsync,
            IAssignUserToOpportunityService assignUserToOpportunityService, IRepositoryAsync<Department> clientDepartmentRepositoryAsync,
            IJwtService jwtService, IRepositoryAsync<User> userRepositoryAsync)
        {
            _repositoryHNAsync = repositoryHNAsync;
            _repositoryAsync = repositoryAsync;
            _mapper = mapper;
            _registerClientService = registerClientService;
            _departmentRepositoryAsync = departmentRepositoryAsync;
            _assignUserToOpportunityService = assignUserToOpportunityService;
            _clientDepartmentRepositoryAsync = clientDepartmentRepositoryAsync;
            _jwtService = jwtService;
            _userRepositoryAsync = userRepositoryAsync;
        }

        public async Task<Response<CustomerDto>> Handle(ImportCustomerCommand request, CancellationToken cancellationToken)
        {
            Guid guid = _jwtService.GetUidToken();
            var checkUser = await _userRepositoryAsync.FirstOrDefaultAsync(new UserIncludesSpecification(guid, null));
            if (checkUser == null)
            {
                throw new ApiException("Usuario invalido");
            }
            var checkIfClientExist = await _repositoryHNAsync.GetByIdAsync(request.Id);
            if (checkIfClientExist == null)
            {
                throw new KeyNotFoundException($"No se encontro el cliente con id {request.Id}");
            }
            var checkIfMotorsCustomerExist = await _repositoryAsync.FirstOrDefaultAsync(new FilterCustomerByMasterIdSpecification(request.Id));
            if (checkIfMotorsCustomerExist != null)
            {
                throw new ApiException("Este cliente ya se encuentra registrado en Motors");
            }
            Guid? assignedUserId;
            if (checkUser.Role!.Name != "Sales Advisor")
            {
                if (checkIfClientExist.DepartmentId != null)
                {
                    var checkDepartment = await _clientDepartmentRepositoryAsync.GetByIdAsync((int)checkIfClientExist.DepartmentId);
                    var getDepartment = await _departmentRepositoryAsync.FirstOrDefaultAsync(new FilterDepartmentFromNameSpecification(checkDepartment!.Name, null));
                    if (getDepartment != null)
                    {
                        assignedUserId = await _assignUserToOpportunityService.FindValidDepartmentUser(getDepartment.Id);
                    }
                    else
                    {
                        assignedUserId = await _assignUserToOpportunityService.FindValidUser();
                    }
                }
                else
                {
                    assignedUserId = await _assignUserToOpportunityService.FindValidUser();
                }
            }
            else
            {
                assignedUserId = checkUser!.Id;
            }
            bool succeeded = await _registerClientService.RegiterClient(request.Id, assignedUserId);
            if (succeeded)
            {
                var dto = _mapper.Map<CustomerDto>(checkIfClientExist);
                return new Response<CustomerDto>(dto, "Cliente importado exitosamente");
            }
            else
            {
                throw new Exception("Ocurrio un error al tratar de importar este cliente");
            }
        }
    }
}
