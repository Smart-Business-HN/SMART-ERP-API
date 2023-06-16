using Microsoft.Extensions.Options;
using RestSharp;
using SMART.ERP.Application.DTOs.Meta.MetaInteractiveReply.MetaInteractiveButtonReply;
using SMART.ERP.Application.DTOs.Meta.MetaInteractiveReply.MetaInteractiveListReply;
using SMART.ERP.Application.DTOs.Meta.MetaTextReply;
using SMART.ERP.Domain.Settings;
using System.Text.Json;

namespace SMART.ERP.Application.Services.MetaPostService
{
    public class MetaPostService : IMetaPostService
    {
        private readonly MetaSettings _metaSettings;
        public MetaPostService(IOptions<MetaSettings> metaSettings)
        {
            _metaSettings = metaSettings.Value;
        }

        public async Task<bool> SendInteractiveButton(MetaInteractiveButtonRoot buttonReply)
        {
            var jsonString = JsonSerializer.Serialize(buttonReply);

            var options = new RestClientOptions(_metaSettings.BaseUrl);
            using var client = new RestClient(options);
            var restRequest = new RestRequest()
                .AddHeader("Authorization", "Bearer " + _metaSettings.Key)
                .AddHeader("Content-Type", "application/json")
                .AddJsonBody(jsonString);
            try
            {
                var response = await client.PostAsync(restRequest);
                var status = response.StatusCode;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> SendInteractiveList(MetaInteractive listResponse)
        {
            var jsonString = JsonSerializer.Serialize(listResponse);

            var options = new RestClientOptions(_metaSettings.BaseUrl);
            using var client = new RestClient(options);
            var restRequest = new RestRequest()
                .AddHeader("Authorization", "Bearer " + _metaSettings.Key)
                .AddHeader("Content-Type", "application/json")
                .AddJsonBody(jsonString);
            try
            {
                var response = await client.PostAsync(restRequest);
                var status = response.StatusCode;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> SendText(MetaResponse response)
        {
            var jsonString = JsonSerializer.Serialize(response);

            var options = new RestClientOptions(_metaSettings.BaseUrl);
            using var client = new RestClient(options);
            var restRequest = new RestRequest()
                .AddHeader("Authorization", "Bearer " + _metaSettings.Key)
                .AddHeader("Content-Type", "application/json")
                .AddJsonBody(jsonString);
            try
            {
                var metaResponse = await client.PostAsync(restRequest);
                var status = metaResponse.StatusCode;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
