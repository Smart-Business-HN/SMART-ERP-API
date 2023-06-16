using MediatR;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.HeaderService;
using SMART.ERP.Application.Specifications.ClientSpecification;
using SMART.ERP.Application.Specifications.CustomerSpecification;
using SMART.ERP.Application.Specifications.WishListSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.MASTER.Domain.Entities;
using SMART.ERP.Application.DTOs.Auth;
using SMART.ERP.Domain.Settings;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace SMART.ERP.Application.Features.CustomerFeature.Commands.LoginCustomerCommand
{
    public class LoginCustomerCommand : IRequest<Response<SesionCustomerDto>>
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }

    public class LoginCustomerCommandHandler : IRequestHandler<LoginCustomerCommand, Response<SesionCustomerDto>>
    {
        private readonly IRepositoryHNAsync<Client> _repositoryHNAsync;
        private readonly IRepositoryAsync<Customer> _repositoryAsync;
        private readonly IHeaderService _headerService;
        private readonly IRepositoryAsync<WishList> _wishListRepositoryAsync;
        private readonly JWTSettings _jwtSettings;

        public LoginCustomerCommandHandler(IRepositoryHNAsync<Client> repositoryHNAsync, IRepositoryAsync<WishList> wishListRepositoryAsync,
            IOptions<JWTSettings> jwtSettings, IRepositoryAsync<Customer> repositoryAsync, IHeaderService headerService)
        {
            _wishListRepositoryAsync = wishListRepositoryAsync;
            _repositoryHNAsync = repositoryHNAsync;
            _repositoryAsync = repositoryAsync;
            _headerService = headerService;
            _jwtSettings = jwtSettings.Value;
        }
        public async Task<Response<SesionCustomerDto>> Handle(LoginCustomerCommand request, CancellationToken cancellationToken)
        {
            var customer = await _repositoryHNAsync.FirstOrDefaultAsync(
                new FilterClientFromEmailSpecification(request.Email));
            if (customer == null)
            {
                throw new ApiException($"No se encontro ningun usuario con el correo {request.Email}");
            }
            var result = await Authenticated(customer, request);
            var verificateCustomer = await _repositoryAsync.FirstOrDefaultAsync(
                new FilterCustomerByMasterIdSpecification(customer.Id));
            if (verificateCustomer == null)
            {
                var registerCustomer = new Customer();
                registerCustomer!.RegistrationDate = DateTime.Now;
                registerCustomer!.MasterId = customer.Id;
                await _repositoryAsync.AddAsync(registerCustomer);
                await _repositoryAsync.SaveChangesAsync();
            }

            return new Response<SesionCustomerDto>(result, message: $"Bienvenido {customer.FullName}");
        }

        private static bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            if (storedHash.Length != 64) throw new ArgumentException("Invalid length of password hash (64 bytes expected).", "passwordHash");
            if (storedSalt.Length != 128) throw new ArgumentException("Invalid length of password salt (128 bytes expected).", "passwordHash");

            var hmac = new HMACSHA512(storedSalt);

            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != storedHash[i]) return false;
            }

            return true;
        }

        private async Task<SesionCustomerDto> Authenticated(Client customer, LoginCustomerCommand request)
        {
            if (request.Password != null && customer.PasswordHash != null && customer.PasswordSalt != null)
            {
                if (!VerifyPasswordHash(request.Password, customer.PasswordHash, customer.PasswordSalt))
                {
                    throw new ApiException($"Verifique su contraseña");
                }
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.Key);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, customer.FullName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(ClaimTypes.Role, "Customer"),
                    new Claim(JwtRegisteredClaimNames.Email, customer.Email!),
                    new Claim("uid", customer.Id.ToString()),
                    new Claim("ip", _headerService.GetClientIP())
                }),
                //Expires = DateTime.UtcNow.AddMinutes(Int32.Parse(_jwtSettings.DurationInMinutes)),
                Expires = DateTime.UtcNow.AddHours(8),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature),
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);
            var localCustomerId = await GetCustomerId(customer.Id);
            var wishListId = await GetWishList(localCustomerId);

            var sesion = new SesionCustomerDto
            {
                Id = customer.Id,
                FullName = customer.FullName,
                Avatar = customer.Avatar,
                ExpirationDate = tokenDescriptor.Expires.Value,
                Token = tokenString,
                WishListId = wishListId
            };

            return sesion;
        }
        private async Task<int?> GetWishList(Guid localCustomerId)
        {
            var verificateWishList = await _wishListRepositoryAsync.FirstOrDefaultAsync(
                   new FilterWishListByCustomerIdSpecification(localCustomerId));
            if (verificateWishList != null)
            {
                return verificateWishList.Id;
            }
            else { return null; }
        }
        private async Task<Guid> GetCustomerId(Guid customerId)
        {
            var internalCustomer = await _repositoryAsync.FirstOrDefaultAsync(
                   new FilterCustomerByMasterIdSpecification(customerId));

            if (internalCustomer != null)
                return internalCustomer!.Id;
            else
                throw new ApiException("No se encontro el cliente");

        }
    }
}