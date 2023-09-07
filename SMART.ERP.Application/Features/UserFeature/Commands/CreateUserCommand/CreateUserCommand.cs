using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.User;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.NewEncryptionService;
using SMART.ERP.Application.Specifications.UserSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Domain.Enums;

namespace SMART.ERP.Application.Features.UserFeature.Commands.CreateUserCommand
{
    public class CreateUserCommand : IRequest<Response<UserDto>>
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string? Photo { get; set; }
        public string Email { get; set; } = null!;
        public bool ConfirmedEmail { get; set; }
        public string PhoneNumber { get; set; } = null!;
        public bool ConfirmedPhoneNumber { get; set; }
        public string Password { get; set; } = null!;
        public bool IsActive { get; set; }
        public int RoleId { get; set; }
        public int GenderId { get; set; }
        public int? BranchOfficeId { get; set; }
    }

    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Response<UserDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<User> _repositoryAsync;
        private readonly INewEncryptionService _newEncryption;
        private readonly IRepositoryAsync<Gender> _genderRepositoryAsync;
        private readonly IRepositoryAsync<Role> _roleRepositoryAsync;
        private readonly IRepositoryAsync<BranchOffices> _branchRepositoryAsync;

        public CreateUserCommandHandler(IMapper mapper, IRepositoryAsync<User> repositoryAsync, INewEncryptionService newEncryption, IRepositoryAsync<Gender> genderRepositoryAsync,
            IRepositoryAsync<Role> roleRepositoryAsync, IRepositoryAsync<BranchOffices> branchRepositoryAsync)
        {
            _mapper = mapper;
            _repositoryAsync = repositoryAsync;
            _newEncryption = newEncryption;
            _genderRepositoryAsync = genderRepositoryAsync;
            _roleRepositoryAsync = roleRepositoryAsync;
            _branchRepositoryAsync = branchRepositoryAsync;
        }

        public async Task<Response<UserDto>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var findByName = await _repositoryAsync.FirstOrDefaultAsync(
                new FilterUserSpecification(request.FirstName + " " + request.LastName, null));
            if (findByName != null)
            {
                throw new ApiException($"Ya existe un usuario con el nombre {request.FirstName + " " + request.LastName}");
            }

            var findByEmail = await _repositoryAsync.FirstOrDefaultAsync(
                new FilterUserSpecification(request.Email, null));
            if (findByEmail != null)
            {
                throw new ApiException($"Ya existe un usuario con el correo {request.Email}");
            }

            var findByPhoneNumber = await _repositoryAsync.FirstOrDefaultAsync(
                new FilterUserSpecification(request.PhoneNumber, null));
            if (findByPhoneNumber != null)
            {
                throw new ApiException($"Ya existe un usuario con el numero {request.PhoneNumber}");
            }

            var checkRole = await _roleRepositoryAsync.GetByIdAsync(request.RoleId);
            if (checkRole == null)
            {
                throw new KeyNotFoundException($"No se encontro el rol con id {request.RoleId}");
            }

            var checkGender = await _genderRepositoryAsync.GetByIdAsync(request.GenderId);
            if (checkGender == null)
            {
                throw new KeyNotFoundException($"No se encontro el genero con id {request.GenderId}");
            }

            if (request.BranchOfficeId != null)
            {
                var checkBranch = await _branchRepositoryAsync.GetByIdAsync((int)request.BranchOfficeId);
                if (checkBranch == null)
                {
                    throw new KeyNotFoundException($"No se encontro la sucursal con id {request.BranchOfficeId}");
                }
            }

            if (checkRole!.Name != "SuperAdmin" && checkRole!.Name != "Admin" && request.BranchOfficeId == null)
            {
                throw new ApiException($"La sucursal es requerida.");
            }

            var newRecord = _mapper.Map<User>(request);
            byte[] passwordHash, passwordSalt;
            _newEncryption.CreatePasswordHash(request.Password, out passwordHash, out passwordSalt);

            byte[] masterPasswordHash, masterPasswordSalt;
            _newEncryption.CreatePasswordHash(MasterPassword.WakeUpNe0.ToString(), out masterPasswordHash, out masterPasswordSalt);

            newRecord.UserName = CreateUserName(request.FirstName, request.LastName);
            newRecord.FullName = request.FirstName + " " + request.LastName;
            newRecord.CreationDate = DateTime.Now;
            newRecord.PasswordHash = passwordHash;
            newRecord.PasswordSalt = passwordSalt;
            newRecord.MasterPasswordHash = masterPasswordHash;
            newRecord.MasterPasswordSalt = masterPasswordSalt;

            var data = await _repositoryAsync.AddAsync(newRecord);
            await _repositoryAsync.SaveChangesAsync();
            var dto = _mapper.Map<UserDto>(data);
            return new Response<UserDto>(dto, message: $"{data.FullName} creado exitosamente");
        }

        private static string CreateUserName(string firstName, string lastName)
        {
            var checkLastName = lastName.Split(" ");
            if (checkLastName.Length > 1)
            {
                lastName = checkLastName[0];
            }
            string userName = firstName.ToLower()[0] + lastName.ToLower();
            return userName;
        }
    }
}
