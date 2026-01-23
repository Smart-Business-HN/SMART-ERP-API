using MediatR;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.ClientSpecification;
using SMART.ERP.Application.Specifications.OpportunitySpecification;
using SMART.ERP.Application.Specifications.ProspectSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
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
        private readonly IRepositoryAsync<Customer> _repositoryHNAsync;
        private readonly IRepositoryAsync<Customer> _customerRepositoryAsync;
        private readonly IRepositoryAsync<Opportunity> _opportunityRepositoryAsync;

        public AdCampaignRoiQueryHandler(IRepositoryAsync<MetaAdCampaign> repositoryAsync, IRepositoryAsync<Prospect> prospectRepositoryAsync,
            IRepositoryAsync<Customer> repositoryHNAsync, IRepositoryAsync<Customer> customerRepositoryAsync,
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

            // ✓ Optimización: Obtener prospects convertidos de la campaña
            var prospects = await _prospectRepositoryAsync.ListAsync(
                new FilterProspectByCampaignIdSpecification(request.Id, true),
                cancellationToken
            );

            // Si no hay prospects convertidos, retornar resultado vacío inmediatamente
            if (prospects.Count == 0)
            {
                return new Response<MetaAdCampaignRoiDto>(new MetaAdCampaignRoiDto
                {
                    Budget = campaign.Lifetime_Budget,
                    Roi = 0,
                    Total = 0,
                    NumConverted = 0
                });
            }

            // ✓ Optimización: Crear HashSet de prospects para búsqueda O(1)
            // Usamos tupla (FullName, PhoneNumber) como clave única
            var prospectKeys = prospects
                .Select(p => (p.FullName.Trim().ToLower(), p.PhoneNumber.Trim()))
                .ToHashSet();

            // ✓ Optimización: Obtener todos los customers en una sola query
            var customers = await _customerRepositoryAsync.ListAsync(cancellationToken);

            // ✓ Optimización: Filtrar customers que coincidan con prospects usando HashSet O(n)
            var matchedCustomerIds = new HashSet<Guid>();
            foreach (var customer in customers)
            {
                if (customer.FullName != null && customer.PhoneNumber != null)
                {
                    var customerKey = (customer.FullName.Trim().ToLower(), customer.PhoneNumber.Trim());
                    if (prospectKeys.Contains(customerKey))
                    {
                        matchedCustomerIds.Add(customer.Id);
                    }
                }
            }

            // Si no hay customers que coincidan, retornar resultado vacío
            if (matchedCustomerIds.Count == 0)
            {
                return new Response<MetaAdCampaignRoiDto>(new MetaAdCampaignRoiDto
                {
                    Budget = campaign.Lifetime_Budget,
                    Roi = 0,
                    Total = 0,
                    NumConverted = 0
                });
            }

            // ✓ Optimización: Obtener oportunidades ganadas con información del cliente
            var opportunities = await _opportunityRepositoryAsync.ListAsync(
                new FilterWonOpportunitiesWithCustomerInfoSpecification(),
                cancellationToken
            );

            // ✓ Optimización: Calcular total de ventas usando HashSet para búsqueda O(1)
            decimal totalSales = 0;
            foreach (var opportunity in opportunities)
            {
                if (matchedCustomerIds.Contains(opportunity.CustomerId))
                {
                    totalSales += opportunity.Total;
                }
            }

            // Calcular ROI
            var dto = new MetaAdCampaignRoiDto
            {
                Budget = campaign.Lifetime_Budget,
                Total = totalSales,
                NumConverted = matchedCustomerIds.Count
            };

            if (dto.Budget > 0)
            {
                dto.Roi = Math.Round(dto.Total / dto.Budget * 100, 2);
            }
            else
            {
                dto.Roi = dto.Total > 0 ? 100 : 0;
            }

            return new Response<MetaAdCampaignRoiDto>(dto);
        }
    }
}
