using SMART.ERP.Application.DTOs.Auth;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Services.EcommerceTokenService;

public interface IEcommerceTokenService
{
    /// <summary>
    /// Emite el JWT del usuario de ecommerce y arma el DTO de sesion.
    /// Centralizado para que el login por contraseña y el login por Google
    /// produzcan exactamente el mismo token y la misma forma de respuesta.
    /// </summary>
    SessionEcommerceUserDto CreateSession(EcommerceUser user);
}
