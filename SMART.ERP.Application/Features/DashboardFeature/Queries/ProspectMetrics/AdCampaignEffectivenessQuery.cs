using MediatR;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.ProspectSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Application.DTOs.Dashboard;

namespace SMART.ERP.Application.Features.DashboardFeature.Queries.ProspectMetrics
{
    public class AdCampaignEffectivenessQuery : IRequest<Response<PieChartDto>>
    {
        public string Id { get; set; } = null!;
    }

    public class AdCampaignEffectivenessQueryHandler : IRequestHandler<AdCampaignEffectivenessQuery, Response<PieChartDto>>
    {
        private readonly IRepositoryAsync<Prospect> _prospectRepositoryAsync;

        public AdCampaignEffectivenessQueryHandler(IRepositoryAsync<Prospect> prospectRepositoryAsync)
        {
            _prospectRepositoryAsync = prospectRepositoryAsync;
        }

        public async Task<Response<PieChartDto>> Handle(AdCampaignEffectivenessQuery request, CancellationToken cancellationToken)
        {
            var prospects = await _prospectRepositoryAsync.ListAsync(new FilterProspectByCampaignIdSpecification(request.Id, false));
            PieChartDto dto = new PieChartDto();
            dto.labels = new List<string>();
            dto.series = new List<decimal>();
            dto.series.Add(prospects.FindAll(x => x.ProspectStep.Name != "Convertido").Count);
            dto.series.Add(prospects.FindAll(x => x.ProspectStep!.Name == "Convertido").Count);
            dto.labels.Add("Prospectos");
            dto.labels.Add("Convertidos");
            return new Response<PieChartDto>(dto);
        }
    }
}
