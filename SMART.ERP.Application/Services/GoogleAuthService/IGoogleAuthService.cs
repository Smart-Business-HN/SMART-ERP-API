using SMART.ERP.Application.DTOs.Auth;

namespace SMART.ERP.Application.Services.GoogleAuthService;

public interface IGoogleAuthService
{
    /// <summary>
    /// Valida la firma, el emisor, la expiracion y la audiencia de un id_token de Google.
    /// Lanza <see cref="Exceptions.ApiException"/> si el token no es valido.
    /// </summary>
    Task<GoogleUserInfo> ValidateIdTokenAsync(string idToken);
}
