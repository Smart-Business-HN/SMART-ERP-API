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

namespace SMART.ERP.Application.Features.UserFeature.Commands.LoginUserCommand
{
    public class LoginUserCommand : IRequest<Response<SessionUserDto>>
    {
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string Password { get; set; } = null!;
    }

    public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, Response<SessionUserDto>>
    {
        private readonly IRepositoryAsync<User> _repositoryAsync;
        private readonly JWTSettings _jwtSettings;
        private readonly IRepositoryAsync<LogSession> _logSessionRepositoryAsync;
        private readonly IRepositoryAsync<BranchOffices> _branchOfficeRepositoryAsync;
        private readonly IRepositoryAsync<RefreshToken> _refreshTokenRepositoryAsync;
        private readonly IHeaderService _headerService;

        public LoginUserCommandHandler(IRepositoryAsync<User> repositoryAsync,
            IOptions<JWTSettings> jwtSettings,
            IRepositoryAsync<LogSession> logSessionRepositoryAsync,
            IRepositoryAsync<BranchOffices> branchOfficeRepositoryAsync,
            IRepositoryAsync<RefreshToken> refreshTokenRepositoryAsync,
            IHeaderService headerService)
        {
            _repositoryAsync = repositoryAsync;
            _jwtSettings = jwtSettings.Value;
            _logSessionRepositoryAsync = logSessionRepositoryAsync;
            _branchOfficeRepositoryAsync = branchOfficeRepositoryAsync;
            _refreshTokenRepositoryAsync = refreshTokenRepositoryAsync;
            _headerService = headerService;
        }

        public async Task<Response<SessionUserDto>> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            if (request.UserName == null && request.Email == null)
            {
                throw new ApiException($"Debe ingresar su correo o nombre de usuario");
            }
            if (request.UserName != null)
            {
                var userByName = await _repositoryAsync.FirstOrDefaultAsync(
                new FilterUserSpecification(request.UserName, null));
                if (userByName == null)
                {
                    throw new ApiException($"No se encontro ningun usuario con el nombre {request.UserName}");
                }
                var result = await Authenticated(userByName, request);
                await SaveSesionLog(
                    userByName.Id,
                    _headerService.GetClientIP(),
                    _headerService.GetClientDeviceInfo(),
                    result.ExpirationDate,
                    _headerService.GetClientLat(),
                    _headerService.GetClientLng());
                return new Response<SessionUserDto>(result, message: $"Bienvenido {userByName.FullName}");
            }
            else
            {
                var userByEmail = await _repositoryAsync.FirstOrDefaultAsync(
                new UserIncludesSpecification(null, request.Email));
                if (userByEmail == null)
                {
                    throw new ApiException($"No se encontro ningun usuario con el correo {request.Email}");
                }
                var result = await Authenticated(userByEmail, request);
                await SaveSesionLog(
                     userByEmail.Id,
                     _headerService.GetClientIP(),
                     _headerService.GetClientDeviceInfo(),
                     result.ExpirationDate,
                     _headerService.GetClientLat(),
                     _headerService.GetClientLng());

                return new Response<SessionUserDto>(result, message: $"Bienvenido {userByEmail.FullName}");
            }
        }

        public async Task SaveSesionLog(Guid userId, string ip, string device, DateTime exDate, string? lat, string? lng)
        {
            try
            {
                if (await _logSessionRepositoryAsync.AnyAsync(new FilterSesionActiveSpecification(userId, ip)))
                {
                    var logSession = await _logSessionRepositoryAsync.FirstOrDefaultAsync(new FilterSesionActiveSpecification(userId, null));
                    if (logSession != null)
                    {
                        logSession.IsActive = false;
                        await _logSessionRepositoryAsync.UpdateAsync(logSession);
                        await _logSessionRepositoryAsync.SaveChangesAsync();
                        throw new ApiException("Ya existe una sesión activa en esta cuenta");
                    }
                }

                var sesion = new LogSession()
                {
                    UserId = userId,
                    IP = ip,
                    DeviceInfo = device,
                    Lat = lat,
                    Lng = lng,
                    RegisterDate = DateTime.Now,
                    ExpirationDate = exDate.AddMinutes(-20),
                    IsActive = true,
                };
                await _logSessionRepositoryAsync.AddAsync(sesion);
                await _logSessionRepositoryAsync.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new ApiException(ex.Message);
            }
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

        private async Task<SessionUserDto> Authenticated(User user, LoginUserCommand request)
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
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature),
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            var refreshTokenRaw = GenerateRefreshToken();
            var refreshTokenHash = HashToken(refreshTokenRaw);

            var refreshTokenEntity = new RefreshToken
            {
                TokenHash = refreshTokenHash,
                UserId = user.Id,
                CreatedDate = DateTime.UtcNow,
                ExpirationDate = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenDurationInDays),
                CreatedByIp = _headerService.GetClientIP(),
                IsRevoked = false,
            };

            await _refreshTokenRepositoryAsync.AddAsync(refreshTokenEntity);
            await _refreshTokenRepositoryAsync.SaveChangesAsync();

            int mainBranch = 0;
            var branchOffice = await _branchOfficeRepositoryAsync.FirstOrDefaultAsync(new GetMainBranchSpecification());
            if (branchOffice != null)
                mainBranch = branchOffice.Id;

            var sesion = new SessionUserDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Photo = user.Photo,
                UserName = user.UserName,
                BranchOfficeId = user.BranchOfficeId,
                Role = user.Role != null ? user.Role!.Selector : string.Empty,
                ExpirationDate = tokenDescriptor.Expires.Value,
                Token = tokenString,
                RefreshToken = refreshTokenRaw,
                MainBranchOfficeId = mainBranch,
            };

            return sesion;
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
