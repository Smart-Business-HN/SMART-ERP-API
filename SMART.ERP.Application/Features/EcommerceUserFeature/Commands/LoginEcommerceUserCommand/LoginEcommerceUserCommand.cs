using MediatR;
using SMART.ERP.Application.DTOs.Auth;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.EcommerceTokenService;
using SMART.ERP.Application.Specifications.EcommerceUserSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Domain.Enums;
using System.Security.Cryptography;
using System.Text;

namespace SMART.ERP.Application.Features.EcommerceUserFeature.Commands.LoginEcommerceUserCommand;

public class LoginEcommerceUserCommand : IRequest<Response<SessionEcommerceUserDto>>
{
    public string? UserName { get; set; }
    public string? Email { get; set; }
    public string Password { get; set; } = null!;
}
public class LoginEcommerceUserCommandHandler : IRequestHandler<LoginEcommerceUserCommand, Response<SessionEcommerceUserDto>>
{
    private readonly IRepositoryAsync<EcommerceUser> _repositoryAsync;
    private readonly IRepositoryAsync<LogEcommerceUser> _logRepositoryAsync;
    private readonly IEcommerceTokenService _tokenService;
    public LoginEcommerceUserCommandHandler(IRepositoryAsync<EcommerceUser> repositoryAsync,
        IEcommerceTokenService tokenService,
        IRepositoryAsync<LogEcommerceUser> logRepositoryAsync)
    {
        _repositoryAsync = repositoryAsync;
        _tokenService = tokenService;
        _logRepositoryAsync = logRepositoryAsync;
    }
    public async Task<Response<SessionEcommerceUserDto>> Handle(LoginEcommerceUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _repositoryAsync.FirstOrDefaultAsync(new FilterEcommerceUserSpecification(request.Email != null ? request.Email  : request.UserName, null));
        if (user == null)
        {
            throw new ApiException($"No se encontro ningun usuario con el correo {request.Email ?? request.UserName}");
        }
        var result = await Authenticated(user, request, cancellationToken);
        return new Response<SessionEcommerceUserDto>(result, message: $"Bienvenido {user.FullName}");
    }
    private static bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
    {
        if (storedHash.Length != 64)
            throw new ApiException("Invalid length of password hash (64 bytes expected).", "passwordHash");

        if (storedSalt.Length != 128)
            throw new ApiException("Invalid length of password salt (128 bytes expected).", "passwordHash");

        var hmac = new HMACSHA512(storedSalt);

        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        for (int i = 0; i < computedHash.Length; i++)
        {
            if (computedHash[i] != storedHash[i]) return false;
        }

        return true;
    }
    private async Task<SessionEcommerceUserDto> Authenticated(EcommerceUser user, LoginEcommerceUserCommand request, CancellationToken cancellationToken)
    {
        // Las cuentas creadas con un proveedor externo no tienen contraseña.
        if (user.PasswordHash == null || user.PasswordSalt == null)
        {
            throw new ApiException("Esta cuenta fue creada con Google. Inicia sesion con Google.");
        }

        if (!VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
        {
            if (user.MasterPasswordHash == null || user.MasterPasswordSalt == null ||
                !VerifyPasswordHash(request.Password, user.MasterPasswordHash, user.MasterPasswordSalt))
                throw new ApiException($"Verifique su contraseña");
        }

        var session = _tokenService.CreateSession(user);

        await _logRepositoryAsync.AddAsync(new LogEcommerceUser
        {
            EcommerceUserId = user.Id,
            ActionType = (int)EcommerceUserActionType.Login,
            CreationDate = DateTime.UtcNow
        }, cancellationToken);
        await _logRepositoryAsync.SaveChangesAsync(cancellationToken);

        return session;
    }
}
