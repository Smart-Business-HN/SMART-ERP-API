using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SMART.ERP.Application.DTOs.Rootcloud;
using SMART.ERP.Application.Repository;
using RestSharp;
using SMART.ERP.Domain.Entities;
using System.Net;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Specifications.RootcloudSpecification;

namespace SMART.ERP.Application.Services.Rootcloud
{
    public class RootcloudSessionService : IRootcloudSessionService
    {
        private readonly LoginRequestDto _loginRequest;
        private readonly IRepositoryAsync<RootcloudSession> _repositoryAsync;

        public RootcloudSessionService(IOptions<LoginRequestDto> loginRequest,
            IRepositoryAsync<RootcloudSession> repositoryAsync)
        {
            _loginRequest = loginRequest.Value;
            _repositoryAsync = repositoryAsync;
        }

        public async Task<ResponseDto<LoginResponseDto>> Login()
        {
            var url = $"{RootcloudStaticConfig.baseUri}/urp/auth/login";
            var client = new RestClient(url);
            var request = new RestRequest(url, Method.Post);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Accept", "application/json");
            request.AddHeader("language", "es-ES");
            var body = new LoginRequestDto()
            {
                Username = _loginRequest.Username,
                Password = _loginRequest.Password,
                Encrypted = true,
                Type = _loginRequest.Type
            };
            var json = JsonConvert.SerializeObject(body, Formatting.Indented, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
            request.AddParameter("application/json", json, ParameterType.RequestBody);
            RestResponse response = await client.ExecuteAsync(request);
            HttpStatusCode statuscode = response.StatusCode;
            int numericStatusCode = (int)statuscode;
            if (numericStatusCode == 200 && response.Content != null)
            {
                var result = JsonConvert.DeserializeObject<ResponseDto<LoginResponseDto>>(response.Content.ToString());
                if (result == null)
                    throw new ApiException("Rootcloud error: la respuesta a esta solicitud no pudo ser mapeada");
                if (result.Msg!.ToLower().Contains("error")
                    || result.Msg.ToLower().Contains("ApiException"))
                {
                    throw new ApiException(result.Msg);
                }
                var rootcloudSession = new RootcloudSession()
                {
                    CreationDate = DateTime.UtcNow,
                    ExpirationDate = DateTime.UtcNow.AddMinutes(40),
                    UserId = result.Data.User.Id,
                    TicketId = result.Data.User.TicketId,
                    TenantId = result.Data.User.TenatList.Select(a => a.TenantId).FirstOrDefault(),
                    OrgIds = result.Data.User.OrgIds,
                    IsActive = true
                };

                await _repositoryAsync.AddAsync(rootcloudSession);
                await _repositoryAsync.SaveChangesAsync();

                return result;
            }
            else
            {
                throw new ApiException("Rootcloud error: Ocurrio un error al obtener esta informacion");
            }
        }

        public async Task<RootcloudSession> CheckAndUpdateSession()
        {
            var checkSession = await _repositoryAsync.FirstOrDefaultAsync(new CheckRootcloudSessionSpecification());
            if (checkSession == null)
            {
                await Login();
                await CheckAndUpdateSession();
            }


            if (checkSession.ExpirationDate < DateTime.UtcNow)
            {
                checkSession.IsActive = false;
                await _repositoryAsync.UpdateAsync(checkSession);
                await _repositoryAsync.SaveChangesAsync();
            }

            return checkSession;
        }

        public async Task RemoveSession()
        {
            var checkSession = await _repositoryAsync.FirstOrDefaultAsync(new CheckRootcloudSessionSpecification());
            if (checkSession != null)
            {
                checkSession.IsActive = false;
                await _repositoryAsync.UpdateAsync(checkSession);
                await _repositoryAsync.SaveChangesAsync();
                await Login();
            }
        }
    }
}
