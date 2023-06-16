using AutoMapper;
using MediatR;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.JwtService;
using SMART.ERP.Application.Services.NewEncryptionService;
using SMART.ERP.Application.Specifications.ClientSpecification;
using SMART.ERP.Application.Specifications.ClientTypeSpecification;
using SMART.ERP.Application.Specifications.DepartmentSpecification;
using SMART.ERP.Application.Specifications.ProspectSpecification;
using SMART.ERP.Application.Specifications.UserSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.MASTER.Domain.Entities;
using SMART.ERP.Application.DTOs.Customer;
using SMART.ERP.Application.Services.AssignUserToOpportunityService;
using SMART.ERP.Application.Services.RegisterClientService;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace SMART.ERP.Application.Features.CustomerFeature.Commands.CreateCustomerCommand
{
    public class CreateCustomerCommand : IRequest<Response<CustomerDto>>
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public bool? ConfirmedPhoneNumber { get; set; }
        public string? Email { get; set; }
        public bool? ConfirmedEmail { get; set; }
        public int HeadingId { get; set; }
        public int DepartmentId { get; set; }
        public int CountryId { get; set; }
        public int SocialReasonId { get; set; }
    }

    public class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, Response<CustomerDto>>
    {
        private readonly IRepositoryHNAsync<Client> _repositoryAsync;
        private readonly IMapper _mapper;
        private readonly IRepositoryHNAsync<ClientType> _clientTypeRepositoryAsync;
        private readonly IRepositoryHNAsync<ClientDepartment> _clientDepartmentRepositoryAsync;
        private readonly IRepositoryAsync<User> _userRepositoryAsync;
        private readonly IJwtService _jwtService;
        private readonly IRepositoryAsync<Department> _departmentRepositoryAsync;
        private readonly IAssignUserToOpportunityService _assignUserToOpportunityService;
        private readonly IRepositoryAsync<Prospect> _prospectRepositoryAsync;
        private readonly IRepositoryHNAsync<ClientCountry> _clientCountryRepositoryAsync;
        private readonly IRepositoryHNAsync<ClientHeading> _clientHeadingRepositoryAsync;
        private readonly INewEncryptionService _newEncryptionService;
        private readonly IRegisterClientService _registerClient;
        private readonly IRepositoryHNAsync<ClientSocialReason> _clientSocialReasonRepositoryAsync;

        public CreateCustomerCommandHandler(IRepositoryHNAsync<Client> repositoryAsync, IMapper mapper,
            IRepositoryHNAsync<ClientType> clientTypeRepositoryAsync,
            IRepositoryHNAsync<ClientHeading> clientHeadingRepositoryAsync,
            INewEncryptionService newEncryptionService,
            IRegisterClientService registerClient,
            IRepositoryHNAsync<ClientSocialReason> clientSocialReasonRepositoryAsync,
            IRepositoryHNAsync<ClientCountry> clientCountryRepositoryAsync,
            IRepositoryHNAsync<ClientDepartment> clientDepartmentRepositoryAsync,
            IRepositoryAsync<User> userRepositoryAsync,
            IJwtService jwtService,
            IRepositoryAsync<Department> departmentRepositoryAsync,
            IAssignUserToOpportunityService assignUserToOpportunityService,
            IRepositoryAsync<Prospect> prospectRepositoryAsync)
        {
            _clientCountryRepositoryAsync = clientCountryRepositoryAsync;
            _clientDepartmentRepositoryAsync = clientDepartmentRepositoryAsync;
            _userRepositoryAsync = userRepositoryAsync;
            _jwtService = jwtService;
            _departmentRepositoryAsync = departmentRepositoryAsync;
            _assignUserToOpportunityService = assignUserToOpportunityService;
            _prospectRepositoryAsync = prospectRepositoryAsync;
            _repositoryAsync = repositoryAsync;
            _mapper = mapper;
            _clientTypeRepositoryAsync = clientTypeRepositoryAsync;
            _clientHeadingRepositoryAsync = clientHeadingRepositoryAsync;
            _newEncryptionService = newEncryptionService;
            _registerClient = registerClient;
            _clientSocialReasonRepositoryAsync = clientSocialReasonRepositoryAsync;
        }

        public async Task<Response<CustomerDto>> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
        {
            Guid guid = _jwtService.GetUidToken();
            var checkUser = await _userRepositoryAsync.FirstOrDefaultAsync(new UserIncludesSpecification(guid, null));
            if (checkUser == null)
            {
                throw new ApiException("Usuario Invalido");
            }
            if (request.Email != null)
            {
                var checkIfExistEmail = await _repositoryAsync.FirstOrDefaultAsync(new FilterClientFromEmailSpecification(request.Email));
                if (checkIfExistEmail != null)
                {
                    throw new ApiException($"Ya existe un cliente con el correo {request.Email}");
                }
            }
            if (request.PhoneNumber != null)
            {
                var checkIfPhoneExist = await _repositoryAsync.FirstOrDefaultAsync(new FilterClientFromPhoneSpecification(request.PhoneNumber));
                if (checkIfPhoneExist != null)
                {
                    throw new ApiException($"Ya existe un cliente con el telefono {request.PhoneNumber}");
                }
            }

            var checkIfCustomerHeadingExists = await _clientHeadingRepositoryAsync.GetByIdAsync(request.HeadingId);
            if (checkIfCustomerHeadingExists == null)
            {
                throw new ApiException($"No existe un rubro de cliente con id {request.HeadingId}");
            }

            var customerType = await _clientTypeRepositoryAsync.FirstOrDefaultAsync(new FilterClientTypeSpecification("Basico"));
            if (customerType == null)
            {
                throw new KeyNotFoundException($"No se encontro el tipo de cliente");
            }
            var customerCountry = await _clientCountryRepositoryAsync.GetByIdAsync(request.CountryId);
            if (customerCountry == null)
            {
                throw new KeyNotFoundException($"No se encontro el pais con el id ${request.CountryId}");
            }
            var customerDepartment = await _clientDepartmentRepositoryAsync.GetByIdAsync(request.DepartmentId);
            if (customerDepartment == null)
            {
                throw new KeyNotFoundException($"No se encontro el departamento con el id ${request.DepartmentId}");
            }


            var newRecord = _mapper.Map<Client>(request);
            byte[] passwordHash, passwordSalt;
            string tempPassword = RandomString(8);
            _newEncryptionService.CreatePasswordHash(tempPassword, out passwordHash, out passwordSalt);

            var socialReason = await _clientSocialReasonRepositoryAsync.GetByIdAsync(request.SocialReasonId);
            if (socialReason == null)
            {
                throw new KeyNotFoundException($"No se encontro la razon social");
            }

            if (socialReason != null && !string.IsNullOrWhiteSpace(request.FirstName) && !string.IsNullOrWhiteSpace(request.LastName) && socialReason != null)
            {
                newRecord.FullName = socialReason.Name == "Natural" ? request.FirstName + " " + request.LastName : request.FullName!;
            }

            var checkFields = await _prospectRepositoryAsync.FirstOrDefaultAsync(new FilterProspectByUniqueFieldsSpecification(
                newRecord.FullName, request.PhoneNumber, request.Email, null));
            if (checkFields != null)
            {
                if (checkFields.FullName == newRecord.FullName)
                {
                    throw new ApiException("Ya existe un prospecto con este nombre");
                }
                if (checkFields.Email == request.Email && request.Email != null)
                {
                    throw new ApiException("Ya existe un prospecto con este correo");
                }
                if (checkFields.PhoneNumber == request.PhoneNumber)
                {
                    throw new ApiException("Ya existe un prospecto con este número telefónico");
                }
            }

            if (request.ConfirmedEmail != null)
            {
                newRecord.ConfirmedEmail = (bool)request.ConfirmedEmail;
            }
            if (request.ConfirmedPhoneNumber != null)
            {
                newRecord.ConfirmedPhoneNumber = (bool)request.ConfirmedPhoneNumber;
            }
            newRecord.Company = socialReason!.Name == "Juridica" ? request.FullName : null;
            newRecord.IsActive = true;
            newRecord.PasswordHash = passwordHash;
            newRecord.PasswordSalt = passwordSalt;
            newRecord.CustomerTypeId = customerType.Id;
            newRecord.DepartmentId = customerDepartment.Id;
            newRecord.CountryId = customerCountry.Id;
            newRecord.Email = string.IsNullOrWhiteSpace(newRecord.Email) ? null : request.Email;
            newRecord.PhoneNumber = string.IsNullOrWhiteSpace(newRecord.PhoneNumber) ? null : newRecord.PhoneNumber;
            newRecord.CurrencyId = newRecord.CurrencyId == 0 ? null : newRecord.CurrencyId;

            var response = await _repositoryAsync.AddAsync(newRecord);
            await _repositoryAsync.SaveChangesAsync();

            Guid? assignedUserId;
            if (checkUser.Role!.Name != "Sales Advisor")
            {
                var getDepartment = await _departmentRepositoryAsync.FirstOrDefaultAsync(new FilterDepartmentFromNameSpecification(customerDepartment.Name, null));
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
                assignedUserId = checkUser!.Id;
            }


            if (await _registerClient.RegiterClient(response.Id, assignedUserId))
            {
                var dto = _mapper.Map<CustomerDto>(response);
                return new Response<CustomerDto>(dto);
            }
            else
            {
                throw new ApiException("Ocurrio un error al tratar de guardar este cliente, consulta a soporte tecnico.");
            }
        }

        static string RandomString(int length)
        {
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ12345678901234567890123456789012345678901234";
            StringBuilder res = new StringBuilder();
            while (length-- > 0)
            {
                var rng = RandomNumberGenerator.GetInt32(95);
                res.Append(valid[rng]);
            }
            if (Regex.Match(res.ToString(), @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[a-zA-Z\d]{8,}$").Success)
            {
                return res.ToString();
            }
            else
            {
                return RandomString(8);
            }

        }
    }
}