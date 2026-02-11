using MediatR;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.NewEncryptionService;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Domain.Enums;

namespace SMART.ERP.Application.Features.EcommerceUserFeature.Commands.ChangeEcommerceUserPasswordCommand;

public class ChangeEcommerceUserPasswordCommand : IRequest<Response<string>>
{
    public Guid Id { get; set; }
    public string CurrentPassword { get; set; } = null!;
    public string NewPassword { get; set; } = null!;
}

public class ChangeEcommerceUserPasswordCommandHandler : IRequestHandler<ChangeEcommerceUserPasswordCommand, Response<string>>
{
    private readonly IRepositoryAsync<EcommerceUser> _repositoryAsync;
    private readonly IRepositoryAsync<LogEcommerceUser> _logRepositoryAsync;
    private readonly INewEncryptionService _newEncryption;

    public ChangeEcommerceUserPasswordCommandHandler(
        IRepositoryAsync<EcommerceUser> repositoryAsync,
        IRepositoryAsync<LogEcommerceUser> logRepositoryAsync,
        INewEncryptionService newEncryption)
    {
        _repositoryAsync = repositoryAsync;
        _logRepositoryAsync = logRepositoryAsync;
        _newEncryption = newEncryption;
    }

    public async Task<Response<string>> Handle(ChangeEcommerceUserPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _repositoryAsync.GetByIdAsync(request.Id, cancellationToken);
        if (user == null)
            throw new ApiException("Usuario no encontrado");

        if (!_newEncryption.VerifyPasswordHash(request.CurrentPassword, user.PasswordHash, user.PasswordSalt))
            throw new ApiException("La contraseña actual es incorrecta");

        _newEncryption.CreatePasswordHash(request.NewPassword, out var passwordHash, out var passwordSalt);
        user.PasswordHash = passwordHash;
        user.PasswordSalt = passwordSalt;
        user.LastPasswordChange = DateTime.UtcNow;
        user.ModificationDate = DateTime.UtcNow;

        await _repositoryAsync.UpdateAsync(user, cancellationToken);
        await _repositoryAsync.SaveChangesAsync(cancellationToken);

        await _logRepositoryAsync.AddAsync(new LogEcommerceUser
        {
            EcommerceUserId = user.Id,
            ActionType = (int)EcommerceUserActionType.PasswordChange,
            CreationDate = DateTime.UtcNow
        }, cancellationToken);
        await _logRepositoryAsync.SaveChangesAsync(cancellationToken);

        return new Response<string>("Contraseña actualizada correctamente");
    }
}
