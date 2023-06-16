using MediatR;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.NewEncryptionService;
using SMART.ERP.Application.Specifications.AuthSpecification;
using SMART.ERP.Application.Specifications.UserSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using System.Security.Cryptography;
using System.Text;

namespace SMART.ERP.Application.Features.UserFeature.Commands.ForgotPasswordCommand
{
    public class ForgotPasswordCommand : IRequest<Response<string>>
    {
        public Guid Id { get; set; }
        public string Password { get; set; } = null!;
        public int Code { get; set; }
    }

    public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, Response<string>>
    {
        private readonly IRepositoryAsync<User> _repositoryAsync;
        private readonly INewEncryptionService _newEncryption;
        private readonly IRepositoryAsync<LogRecovery> _logRecoveryRepositoryAsync;

        public ForgotPasswordCommandHandler(IRepositoryAsync<LogRecovery> logRecoveryRepositoryAsync, IRepositoryAsync<User> repositoryAsync,
            INewEncryptionService newEncryption)
        {
            _repositoryAsync = repositoryAsync;
            _newEncryption = newEncryption;
            _logRecoveryRepositoryAsync = logRecoveryRepositoryAsync;
        }

        public async Task<Response<string>> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await _repositoryAsync.GetByIdAsync(request.Id);
            if (user == null)
            {
                throw new KeyNotFoundException("Ocurrio un error al encontrar este usuario");
            }
            var logRecovery = await _logRecoveryRepositoryAsync.ListAsync(new FilterLogRecoveryFromEmailSpecification(user.Email));
            if (logRecovery.Count == 0)
            {
                throw new ApiException("No se encontraron codigos de recuperacion activas");
            }
            for (var i = 0; i < logRecovery.Count; i++)
            {
                logRecovery[i].IsActive = false;
                if (logRecovery[i].Code == request.Code && logRecovery[i].ExpirationDate > DateTime.Now)
                {
                    byte[] PasswordHash, PasswordSalt;
                    _newEncryption.CreatePasswordHash(request.Password, out PasswordHash, out PasswordSalt);
                    user.PasswordHash = PasswordHash;
                    user.PasswordSalt = PasswordSalt;
                    await _logRecoveryRepositoryAsync.UpdateRangeAsync(logRecovery);
                    await _logRecoveryRepositoryAsync.SaveChangesAsync();
                    await _repositoryAsync.UpdateAsync(user);
                    await _repositoryAsync.SaveChangesAsync();
                    return new Response<string>("Contraseña restablecida exitosamente", $"Hola {user.FullName} tu contraseña ha sido restablecida exitosamente");
                }

            }
            throw new KeyNotFoundException("Ocurrio un error con la solicitud de recuperacion");
        }
    }
}
