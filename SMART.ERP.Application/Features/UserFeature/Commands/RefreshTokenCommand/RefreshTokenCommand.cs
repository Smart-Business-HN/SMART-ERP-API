using MediatR;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SMART.ERP.Application.DTOs.Auth;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.HeaderService;
using SMART.ERP.Application.Specifications.BranchOfficeSpecification;
using SMART.ERP.Application.Specifications.RefreshTokenSpecification;
using SMART.ERP.Application.Specifications.UserSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Domain.Settings;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace SMART.ERP.Application.Features.UserFeature.Commands.RefreshTokenCommand
{
    public class RefreshTokenCommand : IRequest<Response<SessionUserDto>>
    {
        public string Token { get; set; } = null!;
        public string RefreshToken { get; set; } = null!;
    }

    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Response<SessionUserDto>>
    {
        private readonly IRepositoryAsync<RefreshToken> _refreshTokenRepositoryAsync;
        private readonly IRepositoryAsync<User> _userRepositoryAsync;
        private readonly IRepositoryAsync<BranchOffices> _branchOfficeRepositoryAsync;
        private readonly JWTSettings _jwtSettings;
        private readonly IHeaderService _headerService;

        public RefreshTokenCommandHandler(
            IRepositoryAsync<RefreshToken> refreshTokenRepositoryAsync,
            IRepositoryAsync<User> userRepositoryAsync,
            IRepositoryAsync<BranchOffices> branchOfficeRepositoryAsync,
            IOptions<JWTSettings> jwtSettings,
            IHeaderService headerService)
        {
            _refreshTokenRepositoryAsync = refreshTokenRepositoryAsync;
            _userRepositoryAsync = userRepositoryAsync;
            _branchOfficeRepositoryAsync = branchOfficeRepositoryAsync;
            _jwtSettings = jwtSettings.Value;
            _headerService = headerService;
        }

        public async Task<Response<SessionUserDto>> Handle(
            RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var principal = GetPrincipalFromExpiredToken(request.Token);
            var uidClaim = principal.Claims.FirstOrDefault(c => c.Type == "uid");
            if (uidClaim == null)
                throw new ApiException("Token de acceso inválido.");

            var userId = Guid.Parse(uidClaim.Value);

            var refreshTokenHash = HashToken(request.RefreshToken);
            var storedToken = await _refreshTokenRepositoryAsync.FirstOrDefaultAsync(
                new FilterRefreshTokenByHashSpecification(refreshTokenHash));

            if (storedToken == null)
            {
                await RevokeTokenFamily(userId, "Posible reutilización de token revocado");
                throw new ApiException("Token de actualización inválido. Todas las sesiones han sido revocadas.");
            }

            if (storedToken.UserId != userId)
                throw new ApiException("El token de actualización no pertenece a este usuario.");

            if (storedToken.ExpirationDate <= DateTime.UtcNow)
                throw new ApiException("El token de actualización ha expirado. Inicie sesión nuevamente.");

            storedToken.IsRevoked = true;
            storedToken.RevokedDate = DateTime.UtcNow;
            storedToken.RevokedReason = "Reemplazado por rotación";

            var user = await _userRepositoryAsync.FirstOrDefaultAsync(
                new UserIncludesSpecification(userId, null));

            if (user == null || !user.IsActive)
                throw new ApiException("El usuario no existe o está inactivo.");

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.Key);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.FullName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(ClaimTypes.Role, user.Role != null ? user.Role!.Selector : string.Empty),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim("uid", user.Id.ToString()),
                    new Claim("ip", _headerService.GetClientIP())
                }),
                Expires = DateTime.UtcNow.AddMinutes(int.Parse(_jwtSettings.DurationInMinutes)),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature),
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
            };
            var newToken = tokenHandler.CreateToken(tokenDescriptor);
            var newTokenString = tokenHandler.WriteToken(newToken);

            var newRefreshTokenRaw = GenerateRefreshToken();
            var newRefreshTokenHash = HashToken(newRefreshTokenRaw);

            storedToken.ReplacedByTokenHash = newRefreshTokenHash;
            await _refreshTokenRepositoryAsync.UpdateAsync(storedToken);

            var newRefreshTokenEntity = new RefreshToken
            {
                TokenHash = newRefreshTokenHash,
                UserId = user.Id,
                CreatedDate = DateTime.UtcNow,
                ExpirationDate = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenDurationInDays),
                CreatedByIp = _headerService.GetClientIP(),
                IsRevoked = false,
            };
            await _refreshTokenRepositoryAsync.AddAsync(newRefreshTokenEntity);
            await _refreshTokenRepositoryAsync.SaveChangesAsync();

            int mainBranch = 0;
            var branchOffice = await _branchOfficeRepositoryAsync.FirstOrDefaultAsync(
                new GetMainBranchSpecification());
            if (branchOffice != null)
                mainBranch = branchOffice.Id;

            var session = new SessionUserDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Photo = user.Photo,
                UserName = user.UserName,
                BranchOfficeId = user.BranchOfficeId,
                Role = user.Role != null ? user.Role!.Selector : string.Empty,
                ExpirationDate = tokenDescriptor.Expires!.Value,
                Token = newTokenString,
                RefreshToken = newRefreshTokenRaw,
                MainBranchOfficeId = mainBranch,
            };

            return new Response<SessionUserDto>(session, message: "Token actualizado correctamente");
        }

        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(_jwtSettings.Key)),
                ValidateIssuer = true,
                ValidIssuer = _jwtSettings.Issuer,
                ValidateAudience = true,
                ValidAudience = _jwtSettings.Audience,
                ValidateLifetime = false,
                ClockSkew = TimeSpan.Zero,
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(
                token, tokenValidationParameters, out SecurityToken securityToken);

            if (securityToken is not JwtSecurityToken jwtToken ||
                !jwtToken.Header.Alg.Equals(
                    SecurityAlgorithms.HmacSha256,
                    StringComparison.InvariantCultureIgnoreCase))
            {
                throw new ApiException("Token de acceso inválido.");
            }

            return principal;
        }

        private async Task RevokeTokenFamily(Guid userId, string reason)
        {
            var tokens = await _refreshTokenRepositoryAsync.ListAsync(
                new FilterRefreshTokenByUserSpecification(userId));
            foreach (var token in tokens)
            {
                token.IsRevoked = true;
                token.RevokedDate = DateTime.UtcNow;
                token.RevokedReason = reason;
                await _refreshTokenRepositoryAsync.UpdateAsync(token);
            }
            if (tokens.Count > 0)
                await _refreshTokenRepositoryAsync.SaveChangesAsync();
        }

        private static string GenerateRefreshToken()
        {
            var randomBytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }

        private static string HashToken(string token)
        {
            var bytes = Encoding.UTF8.GetBytes(token);
            var hash = SHA256.HashData(bytes);
            return Convert.ToHexString(hash).ToLowerInvariant();
        }
    }
}
