using MediatR;
using SMART.ERP.Application.DTOs.Mail;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.MailService;
using SMART.ERP.Application.Services.NewEncryptionService;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.EcommerceUserFeature.Commands.AdminResetEcommerceUserPasswordCommand;

public class AdminResetEcommerceUserPasswordCommand : IRequest<Response<string>>
{
    public Guid Id { get; set; }
    public string Password { get; set; } = null!;
}

public class AdminResetEcommerceUserPasswordCommandHandler : IRequestHandler<AdminResetEcommerceUserPasswordCommand, Response<string>>
{
    private readonly IRepositoryAsync<EcommerceUser> _repositoryAsync;
    private readonly INewEncryptionService _newEncryption;
    private readonly IMailService _mailService;

    public AdminResetEcommerceUserPasswordCommandHandler(
        IRepositoryAsync<EcommerceUser> repositoryAsync,
        INewEncryptionService newEncryption,
        IMailService mailService)
    {
        _repositoryAsync = repositoryAsync;
        _newEncryption = newEncryption;
        _mailService = mailService;
    }

    public async Task<Response<string>> Handle(AdminResetEcommerceUserPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _repositoryAsync.GetByIdAsync(request.Id, cancellationToken);
        if (user == null)
            throw new KeyNotFoundException($"No se encontró ningún usuario con el id {request.Id}");

        _newEncryption.CreatePasswordHash(request.Password, out var passwordHash, out var passwordSalt);
        user.PasswordHash = passwordHash;
        user.PasswordSalt = passwordSalt;
        user.LastPasswordChange = DateTime.UtcNow;
        user.ModificationDate = DateTime.UtcNow;

        await _repositoryAsync.UpdateAsync(user, cancellationToken);
        await _repositoryAsync.SaveChangesAsync(cancellationToken);

        await SendPasswordResetEmail(user, request.Password);

        return new Response<string>(
            $"La contraseña de {user.FullName} se restableció correctamente",
            "Contraseña restablecida correctamente");
    }

    private async Task SendPasswordResetEmail(EcommerceUser user, string newPassword)
    {
        var mailRequest = new MailRequestDto
        {
            ToEmail = user.Email,
            Subject = "Tu contraseña ha sido restablecida - Smart Business",
            Body = $@"
                <h2 style='color:#1976d2; font-family:Poppins,sans-serif;'>Contraseña restablecida</h2>
                <p style='font-size:16px; font-family:Poppins,sans-serif; color:#333;'>
                    Hola <strong>{user.FullName}</strong>, tu contraseña ha sido restablecida por un administrador.
                    A continuación encontrarás tus nuevas credenciales de acceso:
                </p>

                <table style='border-collapse:collapse; width:100%; font-family:Poppins,sans-serif;'>
                    <tr>
                        <td style='border:1px solid #ddd; padding:8px; background:#f5f5f5;'><strong>Usuario</strong></td>
                        <td style='border:1px solid #ddd; padding:8px;'>{user.UserName}</td>
                    </tr>
                    <tr>
                        <td style='border:1px solid #ddd; padding:8px; background:#f5f5f5;'><strong>Nueva contraseña</strong></td>
                        <td style='border:1px solid #ddd; padding:8px;'>{newPassword}</td>
                    </tr>
                </table>

                <p style='font-size:14px; font-family:Poppins,sans-serif; color:#555; margin-top:20px;'>
                    Te recomendamos cambiar tu contraseña después de iniciar sesión.
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
