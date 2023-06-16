using MediatR;
using SMART.ERP.Application.DTOs.Rootcloud;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.MachinerySpecification;
using SMART.ERP.Application.Specifications.RegionSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.MachineryFeature.Queries
{
    public class GetAllChartsQuery : IRequest<Response<MachineryChartDto>>
    {
        public int SubcategoryId { get; set; }
        public int? CountryId { get; set; }
        public int? RegionId { get; set; }
        public int? DepartmentId { get; set; }
        public int Hour { get; set; }
    }

    public class GetAllChartsQueryHandler : IRequestHandler<GetAllChartsQuery, Response<MachineryChartDto>>
    {
        private readonly IRepositoryAsync<Machinery> _repositoryAsync;
        private readonly IRepositoryAsync<Region> _regionRepositoryAsync;
        private readonly IRepositoryAsync<Country> _countryRepositoryAsync;

        public GetAllChartsQueryHandler(IRepositoryAsync<Machinery> repositoryAsync,
            IRepositoryAsync<Region> regionRepositoryAsync, IRepositoryAsync<Country> countryRepositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
            _regionRepositoryAsync = regionRepositoryAsync;
            _countryRepositoryAsync = countryRepositoryAsync;
        }

        public async Task<Response<MachineryChartDto>> Handle(GetAllChartsQuery request, CancellationToken cancellationToken)
        {
            MachineryChartDto machineryChart = new MachineryChartDto();
            MissingForNextMaintenanceDto result = new MissingForNextMaintenanceDto();
            List<MachineryMissingMaintenanceDto> machineriesInfo = new List<MachineryMissingMaintenanceDto>();

            List<Machinery> response = new List<Machinery>();
            string? countryName = null;
            List<string>? provincies = new List<string>();
            if (request.CountryId > 0)
            {
                var country = await _countryRepositoryAsync.GetByIdAsync(request.CountryId);
                if (country != null)
                {
                    countryName = country.Name;
                    var regions = await _regionRepositoryAsync.ListAsync(new GetRegionByIdSpecification(request.CountryId, request.RegionId, request.DepartmentId));
                    if (regions != null)
                    {
                        foreach (var item in regions)
                        {
                            if (item.Departments.Count > 0)
                            {
                                foreach (var element in item.Departments)
                                {
                                    provincies.Add(element.Name);
                                }
                            }
                        }
                    }
                }
            }

            var machineries = await _repositoryAsync.ListAsync(new GetMachineriesByStatusSpecification(request.SubcategoryId, countryName, null, null));
            if (provincies.Count > 0)
            {
                foreach (var item in provincies)
                {
                    var verificate = machineries.Where(a => a.Province == item).ToList();
                    if (verificate.Count > 0)
                    {
                        response.AddRange(verificate);
                    }
                }
                response = response.Where(a => a.MachineryFailureReports
                .LastOrDefault(x => x.MachineryFailure.Name != "Sin falla") != null
                && a.MachineryFailureReports.Count > 0).ToList();
            }
            else
            {
                response = machineries.Where(a => a.MachineryFailureReports
                .LastOrDefault(x => x.MachineryFailure.Name != "Sin falla") != null
                && a.MachineryFailureReports.Count > 0).ToList();
            }

            machineryChart.InternalMaintenances = response.GroupBy(a => a.ActiveMaintenance).Select(a => new InternalMaintenanceDto
            {
                InternalMaintenance = a.Key,
                Quantity = a.Count(),
                Machineries = a.Select(sm => new MachineryMissingMaintenanceDto
                {
                    Customer = sm.Customer,
                    SerialNum = sm.SerialNum,
                    Hourmeter = sm.MachineyRootcloudHistoricals.Count > 0
                    ? sm.MachineyRootcloudHistoricals.First().Hourmeter : 0,
                    NextMaintenance = sm.MachineyRootcloudHistoricals.Count > 0
                    ? sm.MachineyRootcloudHistoricals.First().NextMaintenance : 0,
                    MissingForNextMaintenance = sm.MachineyRootcloudHistoricals.Count > 0
                    ? sm.MachineyRootcloudHistoricals.First().MissingForNextMaintenance : 0,
                    TimestampLocal = sm.MachineyRootcloudHistoricals.Count > 0
                    ? sm.MachineyRootcloudHistoricals.First().TimestampLocal : "N/A",
                    CreationDate = sm.MachineyRootcloudHistoricals.Count > 0
                    ? sm.MachineyRootcloudHistoricals.First().CreationDate : DateTime.Now,
                    SystemRunning = sm.MachineyRootcloudHistoricals.Count > 0
                    ? sm.MachineyRootcloudHistoricals.First().SystemRunning : false,
                    Country = sm.Country,
                    Province = sm.Province
                }).ToList()
            }).ToList();

            machineryChart.MachineriesByFailure = response.GroupBy(a => a.MachineryFailureReports.LastOrDefault().MachineryFailure.Name).Select(a => new MachineriesByFailureDto
            {
                Failure = a.Key,
                Quantity = a.Count(),
                Machineries = a.Select(sm => new MachineryMissingMaintenanceDto
                {
                    Customer = sm.Customer,
                    SerialNum = sm.SerialNum,
                    Hourmeter = sm.MachineyRootcloudHistoricals.Count > 0
                    ? sm.MachineyRootcloudHistoricals.First().Hourmeter : 0,
                    NextMaintenance = sm.MachineyRootcloudHistoricals.Count > 0
                    ? sm.MachineyRootcloudHistoricals.First().NextMaintenance : 0,
                    MissingForNextMaintenance = sm.MachineyRootcloudHistoricals.Count > 0
                    ? sm.MachineyRootcloudHistoricals.First().MissingForNextMaintenance : 0,
                    TimestampLocal = sm.MachineyRootcloudHistoricals.Count > 0
                    ? sm.MachineyRootcloudHistoricals.First().TimestampLocal : "N/A",
                    CreationDate = sm.MachineyRootcloudHistoricals.Count > 0
                    ? sm.MachineyRootcloudHistoricals.First().CreationDate : DateTime.Now,
                    SystemRunning = sm.MachineyRootcloudHistoricals.Count > 0
                    ? sm.MachineyRootcloudHistoricals.First().SystemRunning : false,
                    Country = sm.Country,
                    Province = sm.Province
                }).ToList()
            }).ToList();

            machineryChart.MachineryByProvince = response.GroupBy(a => new { a.Province, a.Country }).Select(a => new MachineryByProvinceDto
            {
                Province = a.Key.Province,
                Country = a.Key.Country,
                Quantity = a.Count(),
                Machineries = a.Select(sm => new MachineryMissingMaintenanceDto
                {
                    Customer = sm.Customer,
                    SerialNum = sm.SerialNum,
                    Hourmeter = sm.MachineyRootcloudHistoricals.Count > 0
                    ? sm.MachineyRootcloudHistoricals.First().Hourmeter : 0,
                    NextMaintenance = sm.MachineyRootcloudHistoricals.Count > 0
                    ? sm.MachineyRootcloudHistoricals.First().NextMaintenance : 0,
                    MissingForNextMaintenance = sm.MachineyRootcloudHistoricals.Count > 0
                    ? sm.MachineyRootcloudHistoricals.First().MissingForNextMaintenance : 0,
                    TimestampLocal = sm.MachineyRootcloudHistoricals.Count > 0
                    ? sm.MachineyRootcloudHistoricals.First().TimestampLocal : "N/A",
                    CreationDate = sm.MachineyRootcloudHistoricals.Count > 0
                    ? sm.MachineyRootcloudHistoricals.First().CreationDate : DateTime.Now,
                    SystemRunning = sm.MachineyRootcloudHistoricals.Count > 0
                    ? sm.MachineyRootcloudHistoricals.First().SystemRunning : false,
                    Country = sm.Country,
                    Province = sm.Province
                }).ToList()
            }).ToList();

            machineryChart.MachineriesByStatus = response.GroupBy(a => a.MachineryFailureReports.LastOrDefault().Status.Name).Select(a => new MachineriesByStatusDto
            {
                Status = a.Key,
                Quantity = a.Count(),
                Machineries = a.Select(sm => new MachineryMissingMaintenanceDto
                {
                    Customer = sm.Customer,
                    SerialNum = sm.SerialNum,
                    Hourmeter = sm.MachineyRootcloudHistoricals.Count > 0
                    ? sm.MachineyRootcloudHistoricals.First().Hourmeter : 0,
                    NextMaintenance = sm.MachineyRootcloudHistoricals.Count > 0
                    ? sm.MachineyRootcloudHistoricals.First().NextMaintenance : 0,
                    MissingForNextMaintenance = sm.MachineyRootcloudHistoricals.Count > 0
                    ? sm.MachineyRootcloudHistoricals.First().MissingForNextMaintenance : 0,
                    TimestampLocal = sm.MachineyRootcloudHistoricals.Count > 0
                    ? sm.MachineyRootcloudHistoricals.First().TimestampLocal : "N/A",
                    CreationDate = sm.MachineyRootcloudHistoricals.Count > 0
                    ? sm.MachineyRootcloudHistoricals.First().CreationDate : DateTime.Now,
                    SystemRunning = sm.MachineyRootcloudHistoricals.Count > 0
                    ? sm.MachineyRootcloudHistoricals.First().SystemRunning : false,
                    Country = sm.Country,
                    Province = sm.Province
                }).ToList()
            }).ToList();

            if (response.Count > 0)
            {
                foreach (var item in response)
                {
                    if (item.MachineyRootcloudHistoricals.Count > 0)
                    {
                        var checkHourMeter = item.MachineyRootcloudHistoricals.LastOrDefault(s => s.MissingForNextMaintenance < request.Hour);
                        if (checkHourMeter != null)
                        {
                            var machinery = new MachineryMissingMaintenanceDto()
                            {
                                SerialNum = item.SerialNum,
                                Customer = item.Customer,
                                Hourmeter = checkHourMeter.Hourmeter,
                                MissingForNextMaintenance = checkHourMeter.MissingForNextMaintenance,
                                TimestampLocal = checkHourMeter.TimestampLocal,
                                NextMaintenance = checkHourMeter.NextMaintenance,
                                SystemRunning = checkHourMeter.SystemRunning,
                                CreationDate = checkHourMeter.CreationDate,
                                Country = item.Country,
                                Province = item.Province
                            };
                            result.TotalMissing++;
                            machineriesInfo.Add(machinery);
                        }
                    }
                }
                result.TotalNotMissing = result.TotalMachineries - result.TotalMissing;
                result.Machineries = machineriesInfo;
            }

            return new Response<MachineryChartDto>(machineryChart);
        }
    }
}
