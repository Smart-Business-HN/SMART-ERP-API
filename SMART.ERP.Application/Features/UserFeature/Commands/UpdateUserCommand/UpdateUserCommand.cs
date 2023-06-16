using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.User;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.UserSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.UserFeature.Commands.UpdateUserCommand
{
    public class UpdateUserCommand : IRequest<Response<UserDto>>
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string? Photo { get; set; }
        public string Email { get; set; } = null!;
        public bool ConfirmedEmail { get; set; }
        public string PhoneNumber { get; set; } = null!;
        public bool ConfirmedPhoneNumber { get; set; }
        public bool IsActive { get; set; }
        public int RoleId { get; set; }
        public int GenderId { get; set; }
        public int? BranchOfficeId { get; set; }
    }

    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, Response<UserDto>>
    {
        private readonly IRepositoryAsync<User> _repositoryAsync;
        private readonly IRepositoryAsync<City> _cityRepositoryAsync;
        private readonly IRepositoryAsync<Gender> _genderRepositoryAsync;
        private readonly IRepositoryAsync<Role> _roleRepositoryAsync;
        private readonly IMapper _mapper;

        public UpdateUserCommandHandler(IMapper mapper, IRepositoryAsync<User> repositoryAsync, IRepositoryAsync<City> cityRepositoryAsync,
            IRepositoryAsync<Gender> genderRepositoryAsync, IRepositoryAsync<Role> roleRepositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
            _cityRepositoryAsync = cityRepositoryAsync;
            _genderRepositoryAsync = genderRepositoryAsync;
            _roleRepositoryAsync = roleRepositoryAsync;
            _mapper = mapper;
        }

        public async Task<Response<UserDto>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _repositoryAsync.GetByIdAsync(request.Id);
            if (user == null)
            {
                throw new KeyNotFoundException($"No se encontro ningun registro con el id {request.Id}");
            }
            var findByName = await _repositoryAsync.FirstOrDefaultAsync(
                new FilterUserSpecification(request.FirstName + " " + request.LastName, request.Id));
            if (findByName != null)
            {
                throw new ApiException($"Ya existe un usuario con el nombre {request.FirstName + " " + request.LastName}");
            }

            var findByEmail = await _repositoryAsync.FirstOrDefaultAsync(
                new FilterUserSpecification(request.Email, request.Id));
            if (findByEmail != null)
            {
                throw new ApiException($"Ya existe un usuario con el correo {request.Email}");
            }

            var findByPhoneNumber = await _repositoryAsync.FirstOrDefaultAsync(
                new FilterUserSpecification(request.PhoneNumber, request.Id));
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

            if (checkRole!.Name != "SuperAdmin" && checkRole!.Name != "Admin" && request.BranchOfficeId == null)
            {
                throw new ApiException("La sucursal es requerida");
            }

            user.GenderId = request.GenderId;
            user.RoleId = request.RoleId;
            user.BranchOfficeId = request.BranchOfficeId;
            user.PhoneNumber = request.PhoneNumber;
            user.Email = request.Email;
            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.IsActive = request.IsActive;
            user.FullName = request.FirstName + " " + request.LastName;
            user.Photo = request.Photo;
            user.ConfirmedEmail = request.ConfirmedEmail;
            user.ConfirmedPhoneNumber = request.ConfirmedPhoneNumber;
            user.UserName = CreateUserName(request.FirstName, request.LastName);
            user.ModificationDate = DateTime.Now;
            await _repositoryAsync.UpdateAsync(user);
            await _repositoryAsync.SaveChangesAsync();
            var dto = _mapper.Map<UserDto>(user);
            return new Response<UserDto>(dto, message: $"{user.FullName} actualizado correctamente");
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
