using Google.Apis.Auth;
using Microsoft.Extensions.Options;
using SMART.ERP.Application.DTOs.Auth;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Domain.Settings;

namespace SMART.ERP.Application.Services.GoogleAuthService;

public class GoogleAuthService : IGoogleAuthService
{
    private readonly GoogleAuthSettings _settings;

    public GoogleAuthService(IOptions<GoogleAuthSettings> settings)
    {
        _settings = settings.Value;
    }

    public async Task<GoogleUserInfo> ValidateIdTokenAsync(string idToken)
    {
        if (string.IsNullOrWhiteSpace(_settings.ClientId))
        {
            throw new ApiException("El inicio de sesion con Google no esta configurado.");
        }

        try
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = new[] { _settings.ClientId }.Concat(_settings.AdditionalClientIds).ToArray(),
                // Tolerancia de reloj: sin esto, un servidor desfasado unos segundos
                // rechaza tokens recien emitidos con "token used too early".
                IssuedAtClockTolerance = TimeSpan.FromMinutes(5),
                ExpirationTimeClockTolerance = TimeSpan.FromMinutes(5)
            };

            // Verifica firma contra las llaves publicas de Google, ademas de iss y exp.
            var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);

            return new GoogleUserInfo
            {
                Email = payload.Email,
                EmailVerified = payload.EmailVerified,
                FirstName = payload.GivenName ?? payload.Name?.Split(' ').FirstOrDefault() ?? "",
                LastName = payload.FamilyName ?? payload.Name?.Split(' ').Skip(1).FirstOrDefault() ?? "",
                SubjectId = payload.Subject,
                PictureUrl = payload.Picture
            };
        }
        catch (InvalidJwtException)
        {
            throw new ApiException("El token de Google no es valido o ha expirado.");
        }
    }
}
