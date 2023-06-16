using MediatR;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.ClientSpecification;
using SMART.ERP.Application.Specifications.OpportunitySpecification;
using SMART.ERP.Application.Specifications.ProspectSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.MASTER.Domain.Entities;
using SMART.ERP.Application.DTOs.Dashboard;

namespace SMART.ERP.Application.Features.DashboardFeature.Queries.ProspectMetrics
{
    public class AdCampaignRoiQuery : IRequest<Response<MetaAdCampaignRoiDto>>
    {
        public string Id { get; set; } = null!;
    }

    public class AdCampaignRoiQueryHandler : IRequestHandler<AdCampaignRoiQuery, Response<MetaAdCampaignRoiDto>>
    {
        private readonly IRepositoryAsync<MetaAdCampaign> _repositoryAsync;
        private readonly IRepositoryAsync<Prospect> _prospectRepositoryAsync;
        private readonly IRepositoryHNAsync<Client> _repositoryHNAsync;
        private readonly IRepositoryAsync<Customer> _customerRepositoryAsync;
        private readonly IRepositoryAsync<Opportunity> _opportunityRepositoryAsync;

        public AdCampaignRoiQueryHandler(IRepositoryAsync<MetaAdCampaign> repositoryAsync, IRepositoryAsync<Prospect> prospectRepositoryAsync,
            IRepositoryHNAsync<Client> repositoryHNAsync, IRepositoryAsync<Customer> customerRepositoryAsync,
            IRepositoryAsync<Opportunity> opportunityRepositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
            _prospectRepositoryAsync = prospectRepositoryAsync;
            _repositoryHNAsync = repositoryHNAsync;
            _customerRepositoryAsync = customerRepositoryAsync;
            _opportunityRepositoryAsync = opportunityRepositoryAsync;
        }

        public async Task<Response<MetaAdCampaignRoiDto>> Handle(AdCampaignRoiQuery request, CancellationToken cancellationToken)
        {
            var campaign = await _repositoryAsync.GetByIdAsync(request.Id);
            if (campaign == null)
            {
                throw new KeyNotFoundException($"No se encontro la campaña con Id {request.Id}");
            }
            var prospects = await _prospectRepositoryAsync.ListAsync(new FilterProspectByCampaignIdSpecification(request.Id, true));
            var customers = await _customerRepositoryAsync.ListAsync();
            var clients = await _repositoryHNAsync.ListAsync(new FilterClientFromMotors(customers.Select(x => x.MasterId).ToList()));
            var prospectClients = clients.FindAll(x => prospects.Any(y => y.FullName == x.FullName && x.PhoneNumber == y.PhoneNumber));
            var prospectCustomers = customers.FindAll(x => prospectClients.Any(y => y.Id == x.MasterId));
            var opportunities = await _opportunityRepositoryAsync.ListAsync(new FilterWonOpportunitiesSpecification(null));
            MetaAdCampaignRoiDto dto = new();
            dto.Budget = campaign.Lifetime_Budget;
            dto.NumConverted = prospectCustomers.Count;
            foreach (var prospectCustomer in prospectCustomers)
            {
                var prospectOpportunities = opportunities.FindAll(x => x.CustomerId == prospectCustomer.Id);
                if (prospectOpportunities.Count > 0)
                {
                    dto.Total += prospectOpportunities.Sum(x => x.Total);
                }
            }
            if (dto.Budget > 0)
            {
                dto.Roi = Math.Round(dto.Total / dto.Budget * 100, 2);
            }
            else
            {
                if (dto.Total > 0)
                {
                    dto.Roi = 100;
                }
                else
                {
                    dto.Roi = 0;
                }

            }
            return new Response<MetaAdCampaignRoiDto>(dto);
        }
    }
}
