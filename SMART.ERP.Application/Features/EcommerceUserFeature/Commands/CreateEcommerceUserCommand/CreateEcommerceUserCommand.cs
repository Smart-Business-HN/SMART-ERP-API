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
        private readonly IMailService _mailService;

        public CreateEcommerceUserCommandHandler(
            IMapper mapper,
            IRepositoryAsync<EcommerceUser> repositoryAsync,
            INewEncryptionService newEncryption,
            IMailService mailService)
        {
            _mapper = mapper;
            _repositoryAsync = repositoryAsync;
            _newEncryption = newEncryption;
            _mailService = mailService;
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
            await SendWelcomeEmail(request);
            await SendNotificationToAdmin(newUser);
            var dto = _mapper.Map<EcommerceUserDto>(data);
            return new Response<EcommerceUserDto>(dto);
        }

        private async Task SendNotificationToAdmin(EcommerceUser user)
        {
            var adminMailRequest = new MailRequestDto
            {
                ToEmail = "josec@smartbusiness.site", // Cambia por el correo real del admin
                Subject = "Nuevo usuario registrado en Smart Business",
                Body = $@"
                    <h2 style='color:#1976d2; font-family:Poppins,sans-serif;'>Notificación de nuevo registro</h2>
                    
                    <p style='font-size:16px; font-family:Poppins,sans-serif; color:#333;'>
                        Se ha registrado un nuevo usuario en la plataforma:
                    </p>

                    <table style='border-collapse:collapse; width:100%; font-family:Poppins,sans-serif;'>
                        <tr>
                            <td style='border:1px solid #ddd; padding:8px; background:#f5f5f5;'><strong>Nombre</strong></td>
                            <td style='border:1px solid #ddd; padding:8px;'>{user.FirstName}</td>
                        </tr>
                        <tr>
                            <td style='border:1px solid #ddd; padding:8px; background:#f5f5f5;'><strong>Email</strong></td>
                            <td style='border:1px solid #ddd; padding:8px;'>{user.Email}</td>
                        </tr>
                        <tr>
                            <td style='border:1px solid #ddd; padding:8px; background:#f5f5f5;'><strong>Fecha de registro</strong></td>
                            <td style='border:1px solid #ddd; padding:8px;'>{DateTime.Now:dd/MM/yyyy HH:mm}</td>
                        </tr>
                    </table>

                    <p style='font-size:14px; font-family:Poppins,sans-serif; color:#555; margin-top:20px;'>
                        Ingresa al panel de administración para más detalles.
                    </p>
                "
            };
            await _mailService.SendEmailAsync(adminMailRequest);
        }

        private async Task SendWelcomeEmail(CreateEcommerceUserCommand request)
        {
            var mailRequest = new MailRequestDto();
            mailRequest.ToEmail = request.Email;
            mailRequest.Subject = "¡Bienvenido a Smart Business!";
            mailRequest.Body =@"
                <h2 style='color:#1976d2; font-family:Poppins,sans-serif;'>¡Bienvenido a la familia Smart Business!</h2>
                <p style='font-size:16px; font-family:Poppins,sans-serif; color:#333;'>
                    Gracias por registrarte en nuestra plataforma. 
                    En <strong>Smart Business</strong> nos especializamos en ofrecer soluciones de <strong>cableado estructurado, CCTV, fibra óptica, UPS y equipos de marcas líderes como Hikvision y Ubiquiti</strong>.
                </p>

                <p style='font-size:16px; font-family:Poppins,sans-serif; color:#333;'>
                    Queremos que te sientas en confianza de contar con un equipo comprometido con la innovación, 
                    el respaldo técnico y la calidad en cada proyecto.
                </p>

                <p style='font-size:16px; font-family:Poppins,sans-serif; color:#333;'>
                    A partir de hoy tendrás acceso a nuestras novedades, promociones y el soporte especializado que necesitas 
                    para hacer crecer tu negocio.
                </p>

                <div style='margin:30px 0; text-align:center;'>
                    <a href='https://www.smartbusiness.site' target='_blank' 
                       style='background:#1976d2; color:white; padding:12px 24px; border-radius:6px; 
                              font-family:Poppins,sans-serif; text-decoration:none;'>
                        Visita nuestro sitio web
                    </a>
                </div>

                <p style='font-size:14px; font-family:Poppins,sans-serif; color:#555; text-align:center;'>
                    ¡Estamos aquí para ayudarte a alcanzar tus objetivos tecnológicos!
                </p>

                <p style='font-size:14px; font-family:Poppins,sans-serif; color:#555; text-align:center;'>
                    Equipo de <strong>Smart Business</strong>
                </p>
            ";
            await _mailService.SendEmailAsync(mailRequest);
        }
    }
