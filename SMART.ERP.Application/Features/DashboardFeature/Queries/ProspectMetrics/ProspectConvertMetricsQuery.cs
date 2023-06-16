using MediatR;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.ProspectSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Application.DTOs.Dashboard;

namespace SMART.ERP.Application.Features.DashboardFeature.Queries.ProspectMetrics
{
    public class ProspectConvertMetricsQuery : IRequest<Response<ProspectConvertMetricDto>>
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public class ProspectConvertMetricsQueryHandler : IRequestHandler<ProspectConvertMetricsQuery, Response<ProspectConvertMetricDto>>
    {
        private readonly IRepositoryAsync<Prospect> _repositoryAsync;

        public ProspectConvertMetricsQueryHandler(IRepositoryAsync<Prospect> repositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
        }

        public async Task<Response<ProspectConvertMetricDto>> Handle(ProspectConvertMetricsQuery request, CancellationToken cancellationToken)
        {
            if (request.StartDate.HasValue && request.EndDate.HasValue)
            {
                if (request.StartDate.Value.Date >= request.EndDate.Value.Date)
                {
                    throw new ApiException("La fecha inicial no debe ser mayor a la fecha final");
                }
            }
            var prospects = await _repositoryAsync.ListAsync(new FilterProspectByDateRangeSpecification(request.StartDate, request.EndDate));
            ProspectConvertMetricDto dto = new();
            if (prospects.Count > 0)
            {
                dto.Converted = Math.Round((decimal)prospects.FindAll(x => x.ProspectStep!.Name == "Convertido").Count / prospects.Count, 2) * 100;
                dto.NotQualified = Math.Round((decimal)prospects.FindAll(x => x.ProspectStep!.Name == "No Calificado").Count / prospects.Count, 2) * 100;
                dto.NotConverted = Math.Round((decimal)prospects.FindAll(x => x.ProspectStep!.Name != "Convertido").Count / prospects.Count, 2) * 100;
            }
            return new Response<ProspectConvertMetricDto>(dto);
        }
    }
}
