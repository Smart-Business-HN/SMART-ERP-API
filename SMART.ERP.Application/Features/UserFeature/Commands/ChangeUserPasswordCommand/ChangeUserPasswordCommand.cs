using MediatR;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.NewEncryptionService;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.UserFeature.Commands.ChangeUserPasswordCommand
{
    public class ChangeUserPasswordCommand : IRequest<Response<string>>
    {
        public Guid Id { get; set; }
        public string Password { get; set; } = null!;
    }

    public class ChangeUserPasswordCommandHandler : IRequestHandler<ChangeUserPasswordCommand, Response<string>>
    {
        private readonly IRepositoryAsync<User> _repositoryAsync;
        private readonly INewEncryptionService _newEncryption;

        public ChangeUserPasswordCommandHandler(IRepositoryAsync<User> repositoryAsync, INewEncryptionService newEncryption)
        {
            _repositoryAsync = repositoryAsync;
            _newEncryption = newEncryption;
        }
        public async Task<Response<string>> Handle(ChangeUserPasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await _repositoryAsync.GetByIdAsync(request.Id);
            if (user == null)
            {
                throw new KeyNotFoundException($"No se encontro ningun registro con el id {request.Id}");
            }
            byte[] passwordHash, passwordSalt;
            _newEncryption.CreatePasswordHash(request.Password, out passwordHash, out passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            user.ModificationDate = DateTime.Now;
            await _repositoryAsync.UpdateAsync(user);
            await _repositoryAsync.SaveChangesAsync();
            return new Response<string>($"{user.FullName} tu contraseña se actualizo correctamente",
                "Contraseña actualizada correctamente");
        }
    }
}
