using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.EcommerceUser;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.NewEncryptionService;
using SMART.ERP.Application.Specifications.EcommerceUserSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Domain.Enums;

namespace SMART.ERP.Application.Features.EcommerceUserFeature.Commands.CreateEcommerceUserCommand;

public class CreateEcommerceUserCommand : IRequest<Response<EcommerceUserDto>>
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? PhoneNumber { get; set; }
    public string Password { get; set; } = null!;
    public int GenderId { get; set; }
    public int? DepartmentId { get; set; }
}

    public class CreateEcommerceUserCommandHandler : IRequestHandler<CreateEcommerceUserCommand, Response<EcommerceUserDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<EcommerceUser> _repositoryAsync;
        private readonly INewEncryptionService _newEncryption;

        public CreateEcommerceUserCommandHandler(
            IMapper mapper,
            IRepositoryAsync<EcommerceUser> repositoryAsync,
            INewEncryptionService newEncryption)
        {
            _mapper = mapper;
            _repositoryAsync = repositoryAsync;
            _newEncryption = newEncryption;
        }

        public async Task<Response<EcommerceUserDto>> Handle(CreateEcommerceUserCommand request, CancellationToken cancellationToken)
        {
            var findByEmail = await _repositoryAsync.FirstOrDefaultAsync(new FilterEcommerceUserSpecification(request.Email,null));
            if (findByEmail != null)
            {
                throw new ApiException($"Un usuario con el correo {request.Email} ya existe");
            }
            var newUser = _mapper.Map<EcommerceUser>(request);
            newUser.FullName = request.FirstName + " " + request.LastName;
            newUser.UserName = request.FirstName + "_" + request.LastName + Guid.NewGuid().ToString("N").Substring(0,6);
            _newEncryption.CreatePasswordHash(request.Password, out var passwordHash, out var passwordSalt);
            _newEncryption.CreatePasswordHash(nameof(MasterPassword.WakeUpNe0), out var masterPasswordHash, out var masterPasswordSalt);
            newUser.PasswordHash = passwordHash;
            newUser.PasswordSalt = passwordSalt;
            newUser.MasterPasswordHash = masterPasswordHash;
            newUser.MasterPasswordSalt = masterPasswordSalt;
            newUser.CreationDate = DateTime.UtcNow;
            newUser.CustomerTypeId = (int)CustomerTypeEnum.Basico;
            var data = await _repositoryAsync.AddAsync(newUser, cancellationToken);
            await _repositoryAsync.SaveChangesAsync(cancellationToken);

            var dto = _mapper.Map<EcommerceUserDto>(data);
            return new Response<EcommerceUserDto>(dto);
        }
    }
