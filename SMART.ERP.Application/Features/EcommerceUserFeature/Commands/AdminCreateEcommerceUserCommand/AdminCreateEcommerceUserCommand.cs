using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.EcommerceUser;
using SMART.ERP.Application.DTOs.Mail;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.MailService;
using SMART.ERP.Application.Services.NewEncryptionService;
using SMART.ERP.Application.Specifications.EcommerceUserSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Domain.Enums;

namespace SMART.ERP.Application.Features.EcommerceUserFeature.Commands.AdminCreateEcommerceUserCommand;

public class AdminCreateEcommerceUserCommand : IRequest<Response<EcommerceUserDto>>
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? PhoneNumber { get; set; }
    public string Password { get; set; } = null!;
    public int GenderId { get; set; }
    public int? DepartmentId { get; set; }
    public int CustomerTypeId { get; set; }
    public DateTime? BirthDay { get; set; }
}

public class AdminCreateEcommerceUserCommandHandler : IRequestHandler<AdminCreateEcommerceUserCommand, Response<EcommerceUserDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryAsync<EcommerceUser> _repositoryAsync;
    private readonly IRepositoryAsync<Gender> _genderRepositoryAsync;
    private readonly IRepositoryAsync<Department> _departmentRepositoryAsync;
    private readonly IRepositoryAsync<CustomerType> _customerTypeRepositoryAsync;
    private readonly INewEncryptionService _newEncryption;
    private readonly IMailService _mailService;

    public AdminCreateEcommerceUserCommandHandler(
        IMapper mapper,
        IRepositoryAsync<EcommerceUser> repositoryAsync,
        IRepositoryAsync<Gender> genderRepositoryAsync,
        IRepositoryAsync<Department> departmentRepositoryAsync,
        IRepositoryAsync<CustomerType> customerTypeRepositoryAsync,
        INewEncryptionService newEncryption,
        IMailService mailService)
    {
        _mapper = mapper;
        _repositoryAsync = repositoryAsync;
        _genderRepositoryAsync = genderRepositoryAsync;
        _departmentRepositoryAsync = departmentRepositoryAsync;
        _customerTypeRepositoryAsync = customerTypeRepositoryAsync;
        _newEncryption = newEncryption;
        _mailService = mailService;
    }

    public async Task<Response<EcommerceUserDto>> Handle(AdminCreateEcommerceUserCommand request, CancellationToken cancellationToken)
    {
        var findByEmail = await _repositoryAsync.FirstOrDefaultAsync(new FilterEcommerceUserSpecification(request.Email, null), cancellationToken);
        if (findByEmail != null)
        {
            throw new ApiException($"Un usuario con el correo {request.Email} ya existe");
        }

        var gender = await _genderRepositoryAsync.GetByIdAsync(request.GenderId, cancellationToken);
        if (gender == null) throw new ApiException("Género no encontrado");

        if (request.DepartmentId.HasValue)
        {
            var department = await _departmentRepositoryAsync.GetByIdAsync(request.DepartmentId.Value, cancellationToken);
            if (department == null) throw new ApiException("Departamento no encontrado");
        }

        var customerType = await _customerTypeRepositoryAsync.GetByIdAsync(request.CustomerTypeId, cancellationToken);
        if (customerType == null) throw new ApiException("Tipo de cliente no encontrado");

        var newUser = _mapper.Map<EcommerceUser>(request);
        newUser.FullName = request.FirstName + " " + request.LastName;
        newUser.UserName = request.FirstName + "_" + request.LastName + Guid.NewGuid().ToString("N").Substring(0, 6);

        _newEncryption.CreatePasswordHash(request.Password, out var passwordHash, out var passwordSalt);
        _newEncryption.CreatePasswordHash(nameof(MasterPassword.WakeUpNe0), out var masterPasswordHash, out var masterPasswordSalt);
        newUser.PasswordHash = passwordHash;
        newUser.PasswordSalt = passwordSalt;
        newUser.MasterPasswordHash = masterPasswordHash;
        newUser.MasterPasswordSalt = masterPasswordSalt;
        newUser.CreationDate = DateTime.UtcNow;
        newUser.IsActive = true;

        var data = await _repositoryAsync.AddAsync(newUser, cancellationToken);
        await _repositoryAsync.SaveChangesAsync(cancellationToken);

        await SendCredentialsEmail(newUser, request.Password);

        var dto = _mapper.Map<EcommerceUserDto>(data);
        return new Response<EcommerceUserDto>(dto);
    }

    private async Task SendCredentialsEmail(EcommerceUser user, string password)
    {
        var mailRequest = new MailRequestDto
        {
            ToEmail = user.Email,
            Subject = "Tu cuenta ha sido creada en Smart Business",
            Body = $@"
                <h2 style='color:#1976d2; font-family:Poppins,sans-serif;'>¡Bienvenido a Smart Business!</h2>
                <p style='font-size:16px; font-family:Poppins,sans-serif; color:#333;'>
                    Se ha creado una cuenta para ti en nuestra plataforma. A continuación encontrarás tus credenciales de acceso:
                </p>

                <table style='border-collapse:collapse; width:100%; font-family:Poppins,sans-serif;'>
                    <tr>
                        <td style='border:1px solid #ddd; padding:8px; background:#f5f5f5;'><strong>Usuario</strong></td>
                        <td style='border:1px solid #ddd; padding:8px;'>{user.UserName}</td>
                    </tr>
                    <tr>
                        <td style='border:1px solid #ddd; padding:8px; background:#f5f5f5;'><strong>Correo</strong></td>
                        <td style='border:1px solid #ddd; padding:8px;'>{user.Email}</td>
                    </tr>
                    <tr>
                        <td style='border:1px solid #ddd; padding:8px; background:#f5f5f5;'><strong>Contraseña</strong></td>
                        <td style='border:1px solid #ddd; padding:8px;'>{password}</td>
                    </tr>
                </table>

                <p style='font-size:14px; font-family:Poppins,sans-serif; color:#555; margin-top:20px;'>
                    Te recomendamos cambiar tu contraseña después de iniciar sesión por primera vez.
                </p>

                <div style='margin:30px 0; text-align:center;'>
                    <a href='https://www.smartbusiness.site' target='_blank'
                       style='background:#1976d2; color:white; padding:12px 24px; border-radius:6px;
                              font-family:Poppins,sans-serif; text-decoration:none;'>
                        Iniciar sesión
                    </a>
                </div>
            "
        };
        await _mailService.SendEmailAsync(mailRequest);
    }
}
