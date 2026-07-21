using MediatR;
using SMART.ERP.Application.DTOs.Auth;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.EcommerceTokenService;
using SMART.ERP.Application.Services.GoogleAuthService;
using SMART.ERP.Application.Services.NewEncryptionService;
using SMART.ERP.Application.Specifications.EcommerceUserSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Domain.Enums;

namespace SMART.ERP.Application.Features.EcommerceUserFeature.Commands.GoogleLoginEcommerceUserCommand;

public class GoogleLoginEcommerceUserCommand : IRequest<Response<SessionEcommerceUserDto>>
{
    public string IdToken { get; set; } = null!;

    /// <summary>Paso 2 del alta: solo se envian cuando el usuario aun no existe.</summary>
    public string? PhoneNumber { get; set; }
    public int? GenderId { get; set; }

    /// <summary>Contraseña actual, para vincular Google a una cuenta local existente.</summary>
    public string? Password { get; set; }
}

public class GoogleLoginEcommerceUserCommandHandler
    : IRequestHandler<GoogleLoginEcommerceUserCommand, Response<SessionEcommerceUserDto>>
{
    /// <summary>El correo de Google no tiene cuenta: falta el paso 2 (telefono y genero).</summary>
    public const string ProfileIncompleteCode = "PROFILE_INCOMPLETE";

    /// <summary>El correo ya pertenece a una cuenta con contraseña: hay que vincularla.</summary>
    public const string AccountExistsLocalCode = "ACCOUNT_EXISTS_LOCAL";

    private readonly IRepositoryAsync<EcommerceUser> _repositoryAsync;
    private readonly IRepositoryAsync<LogEcommerceUser> _logRepositoryAsync;
    private readonly IGoogleAuthService _googleAuth;
    private readonly IEcommerceTokenService _tokenService;
    private readonly INewEncryptionService _newEncryption;

    public GoogleLoginEcommerceUserCommandHandler(
        IRepositoryAsync<EcommerceUser> repositoryAsync,
        IRepositoryAsync<LogEcommerceUser> logRepositoryAsync,
        IGoogleAuthService googleAuth,
        IEcommerceTokenService tokenService,
        INewEncryptionService newEncryption)
    {
        _repositoryAsync = repositoryAsync;
        _logRepositoryAsync = logRepositoryAsync;
        _googleAuth = googleAuth;
        _tokenService = tokenService;
        _newEncryption = newEncryption;
    }

    public async Task<Response<SessionEcommerceUserDto>> Handle(
        GoogleLoginEcommerceUserCommand request, CancellationToken cancellationToken)
    {
        var google = await _googleAuth.ValidateIdTokenAsync(request.IdToken);

        // El correo es la llave de identidad. Sin verificacion de Google no se crea
        // ni se empata ninguna cuenta.
        if (!google.EmailVerified)
        {
            throw new ApiException("Tu correo de Google no esta verificado.");
        }

        // 1. Por sub: es el identificador estable de la cuenta de Google.
        var bySubject = await _repositoryAsync.FirstOrDefaultAsync(
            new FilterEcommerceUserByGoogleSubjectSpecification(google.SubjectId), cancellationToken);

        if (bySubject != null)
        {
            return await SignIn(bySubject, google, cancellationToken);
        }

        // 2. Por correo.
        var byEmail = await _repositoryAsync.FirstOrDefaultAsync(
            new FilterEcommerceUserSpecification(google.Email, null), cancellationToken);

        if (byEmail != null)
        {
            return await LinkExistingAccount(byEmail, google, request.Password, cancellationToken);
        }

        // 3. Usuario nuevo.
        return await Register(google, request, cancellationToken);
    }

    /// <summary>
    /// Vincula la identidad de Google a una cuenta que ya existe.
    /// Si la cuenta tiene contraseña se exige comprobarla: el registro con contraseña
    /// nunca verifico la propiedad del correo, asi que vincular sin ese paso permitiria
    /// que quien registro el correo primero conserve acceso a la cuenta del dueño real.
    /// </summary>
    private async Task<Response<SessionEcommerceUserDto>> LinkExistingAccount(
        EcommerceUser user, GoogleUserInfo google, string? password, CancellationToken cancellationToken)
    {
        var hasPassword = user.PasswordHash != null && user.PasswordSalt != null;

        if (hasPassword)
        {
            if (string.IsNullOrEmpty(password))
            {
                return new Response<SessionEcommerceUserDto>
                {
                    Succeeded = false,
                    Message = "Ya tienes una cuenta con este correo. Ingresa tu contraseña para vincularla con Google.",
                    Errors = [AccountExistsLocalCode]
                };
            }

            // Sin el fallback de contraseña maestra: vincular es una operacion sensible.
            if (!_newEncryption.VerifyPasswordHash(password, user.PasswordHash!, user.PasswordSalt!))
            {
                throw new ApiException("La contraseña es incorrecta.");
            }
        }

        user.GoogleSubjectId = google.SubjectId;
        user.EmailVerified = true;
        user.ModificationDate = DateTime.UtcNow;

        await _repositoryAsync.UpdateAsync(user, cancellationToken);
        await _repositoryAsync.SaveChangesAsync(cancellationToken);

        return await SignIn(user, google, cancellationToken);
    }

    private async Task<Response<SessionEcommerceUserDto>> Register(
        GoogleUserInfo google, GoogleLoginEcommerceUserCommand request, CancellationToken cancellationToken)
    {
        // Paso 2: el esquema exige telefono y genero, que Google no entrega.
        if (string.IsNullOrWhiteSpace(request.PhoneNumber) || !request.GenderId.HasValue)
        {
            return new Response<SessionEcommerceUserDto>
            {
                Succeeded = false,
                Message = "Necesitamos un par de datos mas para crear tu cuenta.",
                Errors = [ProfileIncompleteCode],
                // Datos de Google para precargar el formulario del paso 2.
                Data = new SessionEcommerceUserDto
                {
                    FirstName = google.FirstName,
                    LastName = google.LastName,
                    FullName = $"{google.FirstName} {google.LastName}".Trim(),
                    Email = google.Email,
                    Photo = google.PictureUrl
                }
            };
        }

        var newUser = new EcommerceUser
        {
            Email = google.Email,
            FirstName = google.FirstName,
            LastName = google.LastName,
            FullName = $"{google.FirstName} {google.LastName}".Trim(),
            UserName = $"{google.FirstName}_{google.LastName}{Guid.NewGuid().ToString("N")[..6]}",
            Photo = google.PictureUrl,
            PhoneNumber = request.PhoneNumber,
            GenderId = request.GenderId.Value,
            CreationDate = DateTime.UtcNow,
            CustomerTypeId = (int)CustomerTypeEnum.Basico,
            AuthProvider = (int)Domain.Enums.AuthProvider.Google,
            GoogleSubjectId = google.SubjectId,
            EmailVerified = true,
            // IsActive explicito: el alta con contraseña lo deja en false por omision.
            IsActive = true
            // Las columnas de contraseña quedan nulas a proposito. Poblarlas con la
            // contraseña maestra expondria la cuenta al fallback de LoginEcommerceUserCommand.
        };

        var created = await _repositoryAsync.AddAsync(newUser, cancellationToken);
        await _repositoryAsync.SaveChangesAsync(cancellationToken);

        return await SignIn(created, google, cancellationToken);
    }

    private async Task<Response<SessionEcommerceUserDto>> SignIn(
        EcommerceUser user, GoogleUserInfo google, CancellationToken cancellationToken)
    {
        // Sincroniza el avatar solo si no hay uno propio: no pisar una foto que el
        // usuario subio al blob storage.
        if (!string.IsNullOrEmpty(google.PictureUrl) &&
            (string.IsNullOrEmpty(user.Photo) || user.Photo.Contains("googleusercontent.com")) &&
            user.Photo != google.PictureUrl)
        {
            user.Photo = google.PictureUrl;
            await _repositoryAsync.UpdateAsync(user, cancellationToken);
            await _repositoryAsync.SaveChangesAsync(cancellationToken);
        }

        var session = _tokenService.CreateSession(user);

        await _logRepositoryAsync.AddAsync(new LogEcommerceUser
        {
            EcommerceUserId = user.Id,
            ActionType = (int)EcommerceUserActionType.GoogleLogin,
            CreationDate = DateTime.UtcNow
        }, cancellationToken);
        await _logRepositoryAsync.SaveChangesAsync(cancellationToken);

        return new Response<SessionEcommerceUserDto>(session, message: $"Bienvenido {user.FullName}");
    }
}
