using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Net.Http.Headers;
using SMART.ERP.Application.Exceptions;

namespace SMART.ERP.Application.Services.HeaderService
{
    public class HeaderService : IHeaderService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;

        public HeaderService(IHttpContextAccessor httpContextAccessor,
            IConfiguration configuration)
        {
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
        }

        public string GetClientDeviceInfo()
        {
            if (_httpContextAccessor.HttpContext == null)
            {
                throw new ApiException("Ocurrio un problema en los encabezados de esta solicitud.");
            }
            else
            {
                var header = _httpContextAccessor.HttpContext.Request.Headers;
                if (header.ContainsKey("sec-ch-ua"))
                {
                    return header["sec-ch-ua"]!;
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        public string GetClientIP()
        {
            if (_httpContextAccessor.HttpContext == null)
            {
                throw new ApiException("Ocurrio un problema en los encabezados de esta solicitud.");
            }
            else
            {
                var header = _httpContextAccessor.HttpContext.Request.Headers;
                if (header.ContainsKey("X-Forwarded-For"))
                    return header["X-Forwarded-For"]!;
                else
                    return _httpContextAccessor.HttpContext.Connection.RemoteIpAddress!.MapToIPv4().ToString();
            }
        }

        public string GetClientLat()
        {
            if (_httpContextAccessor.HttpContext == null)
            {
                throw new ApiException("Ocurrio un problema en los encabezados de esta solicitud.");
            }
            else
            {
                var header = _httpContextAccessor.HttpContext.Request.Headers;
                if (header.ContainsKey("Lat"))
                {
                    return header["Lat"]!;
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        public string GetClientLng()
        {
            if (_httpContextAccessor.HttpContext == null)
            {
                throw new ApiException("Ocurrio un problema en los encabezados de esta solicitud.");
            }
            else
            {
                var header = _httpContextAccessor.HttpContext.Request.Headers;
                if (header.ContainsKey("Lng"))
                {
                    return header["Lng"]!;
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        public bool VerificatedSecretKey()
        {
            if (_httpContextAccessor.HttpContext == null)
            {
                throw new ApiException("Ocurrio un problema en los encabezados de esta solicitud.");
            }
            else
            {
                var token = _httpContextAccessor.HttpContext.Request.Headers[HeaderNames.Authorization].ToString();
                if (!string.IsNullOrEmpty(token))
                {
                    return true;
                }
                else
                {
                    var header = _httpContextAccessor.HttpContext.Request.Headers;
                    if (header.ContainsKey("secret-key"))
                    {
                        if (header["secret-key"].ToString() == _configuration.GetValue<string>("AccessKey")!.ToString())
                            return true;
                    }

                    return false;
                }
            }
        }
    }
}
