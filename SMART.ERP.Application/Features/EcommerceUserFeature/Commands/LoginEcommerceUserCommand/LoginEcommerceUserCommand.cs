using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using MediatR;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SMART.ERP.Application.DTOs.Auth;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.CartSpecification;
using SMART.ERP.Application.Specifications.EcommerceUserSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Domain.Settings;

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
    private readonly JWTSettings _jwtSettings;
    //private readonly IRepositoryAsync<LogSession> _logSessionRepositoryAsync;
    public LoginEcommerceUserCommandHandler(IRepositoryAsync<EcommerceUser> repositoryAsync,
        IOptions<JWTSettings> jwtSettings
        //IRepositoryAsync<LogSession> logSessionRepositoryAsync
        )
    {
        _repositoryAsync = repositoryAsync;
        _jwtSettings = jwtSettings.Value;
        //_logSessionRepositoryAsync = logSessionRepositoryAsync;
    }
    public async Task<Response<SessionEcommerceUserDto>> Handle(LoginEcommerceUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _repositoryAsync.FirstOrDefaultAsync(new FilterEcommerceUserSpecification(request.Email != null ? request.Email  : request.UserName, null));
        if (user == null)
        {
            throw new ApiException($"No se encontro ningun usuario con el correo {request.Email ?? request.UserName}");
        }
        var result = await Authenticated(user, request);
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
        private async Task<SessionEcommerceUserDto> Authenticated(EcommerceUser user, LoginEcommerceUserCommand request)
        {
            if (!VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
            {
                if (!VerifyPasswordHash(request.Password, user.MasterPasswordHash, user.MasterPasswordSalt))
                    throw new ApiException($"Verifique su contraseña");
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.Key);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity([
                    new Claim(JwtRegisteredClaimNames.Sub, user.FullName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim("uid", user.Id.ToString())
                ]),
                //Expires = DateTime.UtcNow.AddMinutes(Int32.Parse(_jwtSettings.DurationInMinutes)),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature),
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token); 
            var session = new SessionEcommerceUserDto
            {
                Id = user.Id,
                FullName = user.FullName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Photo = user.Photo,
                UserName = user.UserName,
                CustomerType = user.CustomerType,
                ExpirationDate = tokenDescriptor.Expires.Value,
                Token = tokenString,
                ActiveCartId = user.Carts!.Any() ? user.Carts!.First(x=>x.IsActive).Id : null
            };

            return session;
        }
}