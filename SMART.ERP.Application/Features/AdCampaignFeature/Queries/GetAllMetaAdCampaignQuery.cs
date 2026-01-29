using AutoMapper;
using MediatR;
using Microsoft.Extensions.Options;
using RestSharp;
using SMART.ERP.Application.DTOs.Meta.MetAdCampaign;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Domain.Settings;
using System.Text.Json;

namespace SMART.ERP.Application.Features.AdCampaignFeature.Queries
{
    public class GetAllMetaAdCampaignQuery : IRequest<Response<List<MetaAdCampaignDto>>>
    {
        public class GetAllMetaAdCampaignQueryHandler : IRequestHandler<GetAllMetaAdCampaignQuery, Response<List<MetaAdCampaignDto>>>
        {
            private readonly IRepositoryAsync<MetaAdCampaign> _repositoryAsync;
            private readonly IMapper _mapper;
            private readonly MetaAdSettings _options;

            public GetAllMetaAdCampaignQueryHandler(IRepositoryAsync<MetaAdCampaign> repositoryAsync, IOptions<MetaAdSettings> options,
                IMapper mapper)
            {
                _repositoryAsync = repositoryAsync;
                _mapper = mapper;
                _options = options.Value;
            }

            public async Task<Response<List<MetaAdCampaignDto>>> Handle(GetAllMetaAdCampaignQuery request, CancellationToken cancellationToken)
            {
                var adCampaigns = await _repositoryAsync.ListAsync();
                var options = new RestClientOptions(_options.BaseUrl);
                using var client = new RestClient(options);
                var restRequest = new RestRequest()
                    .AddHeader("Authorization", "Bearer " + _options.Key)
                    .AddQueryParameter("fields", "name,lifetime_budget,stop_time");
                try
                {
                    var response = await client.GetAsync(restRequest);
                    if (response.Content == null)
                    {
                        throw new ApiException("Ha ocurrido un error al tratar de obtener al campañas publicitarias");
                    }
                    var list = JsonSerializer.Deserialize<MetaAdCampaignReply>(response.Content);
                    var toAddList = new List<MetaAdCampaign>();
                    if (list!.data != null)
                    {
                        foreach (var campaign in list.data)
                        {
                            string[] dateTimeString = campaign.stop_time.Split("-0600");
                            if (!adCampaigns.Exists(x => x.Id == campaign.id))
                            {
                                MetaAdCampaign newRecord = new MetaAdCampaign
                                {
                                    Id = campaign.id,
                                    Name = campaign.name,
                                    Stop_Time = DateTime.Parse(dateTimeString[0])
                                };
                                decimal.TryParse(campaign.lifetime_budget, out decimal result);
                                result = result / 100;
                                newRecord.Lifetime_Budget = result;
                                toAddList.Add(newRecord);
                            }
                        }
                        if (toAddList.Count > 0)
                        {
                            await _repositoryAsync.AddRangeAsync(toAddList);
                            await _repositoryAsync.SaveChangesAsync();

                            adCampaigns = await _repositoryAsync.ListAsync();
                        }
                    }

                    var dto = _mapper.Map<List<MetaAdCampaignDto>>(adCampaigns);
                    return new Response<List<MetaAdCampaignDto>>(dto);
                }
                catch (Exception e)
                {
                    throw new ApiException("Ha ocurrido un error al tratar de obtener al campañas publicitarias");
                }
            }
        }
    }
}
