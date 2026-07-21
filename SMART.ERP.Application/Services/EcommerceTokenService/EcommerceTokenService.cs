using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SMART.ERP.Application.DTOs.Auth;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Domain.Settings;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SMART.ERP.Application.Services.EcommerceTokenService;

public class EcommerceTokenService : IEcommerceTokenService
{
    private readonly JWTSettings _jwtSettings;

    public EcommerceTokenService(IOptions<JWTSettings> jwtSettings)
    {
        _jwtSettings = jwtSettings.Value;
    }

    public SessionEcommerceUserDto CreateSession(EcommerceUser user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        // UTF8, no ASCII: el middleware de validacion (ServiceExtensions) usa UTF8.
        // Con la key actual (ASCII puro) ambos coinciden, pero divergirian al rotarla.
        var key = Encoding.UTF8.GetBytes(_jwtSettings.Key);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity([
                new Claim(JwtRegisteredClaimNames.Sub, user.FullName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("uid", user.Id.ToString())
            ]),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature),
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience,
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);

        return new SessionEcommerceUserDto
        {
            Id = user.Id,
            FullName = user.FullName,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Photo = user.Photo,
            UserName = user.UserName,
            CustomerType = user.CustomerType,
            AuthProvider = user.AuthProvider,
            ExpirationDate = tokenDescriptor.Expires!.Value,
            Token = tokenHandler.WriteToken(token),
            // FirstOrDefault: un usuario puede tener carritos sin ninguno activo.
            ActiveCartId = user.Carts?.FirstOrDefault(x => x.IsActive)?.Id
        };
    }
}
