using MediatR;
using SMART.ERP.Application.DTOs.Rootcloud;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.MachinerySpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.ReportFeature.Queries
{
    public class RootcloudQuery : IRequest<Response<List<ResumeHistoricalDto>>>
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    public class RootcloudQueryHandler : IRequestHandler<RootcloudQuery, Response<List<ResumeHistoricalDto>>>
    {
        private readonly IRepositoryAsync<Machinery> _repositoryAsync;

        public RootcloudQueryHandler(IRepositoryAsync<Machinery> repositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
        }

        public async Task<Response<List<ResumeHistoricalDto>>> Handle(RootcloudQuery request, CancellationToken cancellationToken)
        {
            List<ResumeHistoricalDto> resumeHistoricals = new List<ResumeHistoricalDto>();
            var machineries = await _repositoryAsync.ListAsync(new GetHistoricalByDateSpecification(request.StartDate, request.EndDate));
            if (machineries.Count > 0)
            {
                foreach (var item in machineries)
                {
                    if (item.MachineyRootcloudHistoricals.Count > 1)
                    {
                        var groupByDevice = item.MachineyRootcloudHistoricals
                            .Where(x => x.Hourmeter > 0
                            && x.MissingForNextMaintenance > 0)
                                .Select(s => new ResumeHistoricalDto
                                {
                                    SerialNum = item.SerialNum,
                                    Customer = item.Customer,
                                    Country = item.Country,
                                    Province = item.Province,
                                    MachineTypeName = item.MachineTypeName,
                                    InitHourmeter = item.MachineyRootcloudHistoricals.OrderBy(o => o.Id).Skip(1).First().Hourmeter,
                                    EndHourmeter = item.MachineyRootcloudHistoricals.OrderBy(o => o.Id).Last().Hourmeter,
                                    InitMilenage = item.MachineyRootcloudHistoricals.OrderBy(o => o.Id).Skip(1).First().Milenage,
                                    EndMilenage = item.MachineyRootcloudHistoricals.OrderBy(o => o.Id).Last().Milenage,
                                    NextMaintenance = item.MachineyRootcloudHistoricals.OrderBy(o => o.Id).Last().NextMaintenance,
                                    MissingForNextMaintenance = item.MachineyRootcloudHistoricals.OrderBy(o => o.Id).Last().MissingForNextMaintenance,
                                    AvarageHourmeter = decimal.Truncate(item.MachineyRootcloudHistoricals
                                    .Zip(item.MachineyRootcloudHistoricals.Skip(1), (a, b) => b.Hourmeter - a.Hourmeter).Average()),
                                    MissingDaysForNextMaintenance = decimal.Truncate(item.MachineyRootcloudHistoricals
                                    .OrderBy(o => o.Id).Skip(1).First().Hourmeter == item.MachineyRootcloudHistoricals
                                    .OrderBy(o => o.Id).Last().Hourmeter ? 0
                                    : item.MachineyRootcloudHistoricals
                                    .OrderBy(o => o.Id).Last().MissingForNextMaintenance / item.MachineyRootcloudHistoricals
                                    .Zip(item.MachineyRootcloudHistoricals.Skip(1), (a, b) => b.Hourmeter - a.Hourmeter).Average()),
                                }).ToList();
                        resumeHistoricals = groupByDevice;
                    }
                }
            }

            return new Response<List<ResumeHistoricalDto>>(resumeHistoricals);
        }
    }
}
