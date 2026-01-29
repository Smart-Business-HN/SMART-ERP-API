using MediatR;
using SMART.ERP.Application.DTOs.Auth;
using SMART.ERP.Application.DTOs.Mail;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.MailService;
using SMART.ERP.Application.Specifications.AuthSpecification;
using SMART.ERP.Application.Specifications.UserSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.UserFeature.Commands.CreateForgotCodeCommand
{
    public class CreateForgotCodeCommand : IRequest<Response<RecoveryCodeDto>>
    {
        public string Email { get; set; } = null!;
    }

    public class CreateForgotCodeCommandHandler : IRequestHandler<CreateForgotCodeCommand, Response<RecoveryCodeDto>>
    {
        private readonly IRepositoryAsync<User> _repositoryAsync;
        private readonly IMailService _mailService;
        private readonly IRepositoryAsync<LogRecovery> _logRecoveryRepositoryAsync;

        public CreateForgotCodeCommandHandler(IRepositoryAsync<User> repositoryAsync, IMailService mailService, IRepositoryAsync<LogRecovery> logRecoveryRepositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
            _mailService = mailService;
            _logRecoveryRepositoryAsync = logRecoveryRepositoryAsync;
        }

        public async Task<Response<RecoveryCodeDto>> Handle(CreateForgotCodeCommand request, CancellationToken cancellationToken)
        {
            var user = await _repositoryAsync.FirstOrDefaultAsync(new FilterUserSpecification(request.Email, null));
            if (user == null)
            {
                throw new KeyNotFoundException($"Registro no encontrado con el correo {request.Email}");

            }
            var logRecoveryList = _logRecoveryRepositoryAsync.ListAsync(new FilterLogRecoveryFromDateToday(request.Email, DateTime.Now));
            if (logRecoveryList.Result.Count >= 3)
            {
                throw new ApiException("Solamente puede solicitar un maximo de tres codigos por dia");
            }
            Random code = new Random();
            var confirmCode = code.Next(1000, 9999);
            var result = new RecoveryCodeDto
            {
                Id = user.Id,
                Code = confirmCode,
                Expiration = DateTime.Now.AddMinutes(15)
            };
            var logRecovery = new LogRecovery
            {
                Email = user.Email,
                Code = confirmCode,
                ExpirationDate = DateTime.Now.AddMinutes(15),
                IsActive = true
            };
            string amOrPm = result.Expiration.Hour > 12 ? "pm" : "am";
            MailRequestDto mailRequest = new MailRequestDto()
            {
                ToEmail = user.Email,
                Subject = "Restablecer contraseña " + result.Expiration.ToString("hh:mm:s") + " " + amOrPm,
                Body = "Hola de nuevo " + user.FullName +
                ",<br><br> Para restablecer tu contraseña deberas ingresar el siguiente código de seguridad: <br><br>"
                + "<div style=\"text-align:center;font-size:30px;font-size:40px;font-weight:bold;\">" + confirmCode + "</div>" + "<br><br> El cual vence a las " + result.Expiration.ToString("hh:mm:s") + " " + amOrPm +
                " del dia de hoy. <br><br> Si no solicitaste este código, puedes hacer caso omiso de este " +
                "mensaje de correo electrónico y si el código de confirmación no funciona notifica al departamento de IT",
            };
            await _mailService.SendEmailAsync(mailRequest);
            await _logRecoveryRepositoryAsync.AddAsync(logRecovery);
            return new Response<RecoveryCodeDto>(result, message: $"Codigo enviado exitosamente");
        }
    }
}
