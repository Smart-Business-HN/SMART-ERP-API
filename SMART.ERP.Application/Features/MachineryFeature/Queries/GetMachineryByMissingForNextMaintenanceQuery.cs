using MediatR;
using SMART.ERP.Application.DTOs.Rootcloud;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.MachinerySpecification;
using SMART.ERP.Application.Specifications.RegionSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.MachineryFeature.Queries
{
    public class GetMachineryByMissingForNextMaintenanceQuery : IRequest<Response<MissingForNextMaintenanceDto>>
    {
        public int Hour { get; set; }
        public int SubcategoryId { get; set; }
        public int? CountryId { get; set; }
        public int? RegionId { get; set; }
        public int? DepartmentId { get; set; }
        public int? BrandId { get; set; }
        public string? Status { get; set; }
    }
    public class GetMachineryByMissingForNextMaintenanceQueryHandler
            : IRequestHandler<GetMachineryByMissingForNextMaintenanceQuery, Response<MissingForNextMaintenanceDto>>
    {
        private readonly IRepositoryAsync<Machinery> _repositoryAsync;
        private readonly IRepositoryAsync<Region> _regionRepositoryAsync;
        private readonly IRepositoryAsync<Country> _countryRepositoryAsync;

        public GetMachineryByMissingForNextMaintenanceQueryHandler(IRepositoryAsync<Machinery> repositoryAsync,
            IRepositoryAsync<Region> regionRepositoryAsync, IRepositoryAsync<Country> countryRepositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
            _regionRepositoryAsync = regionRepositoryAsync;
            _countryRepositoryAsync = countryRepositoryAsync;
        }
        public async Task<Response<MissingForNextMaintenanceDto>> Handle(GetMachineryByMissingForNextMaintenanceQuery request, CancellationToken cancellationToken)
        {
            var result = new MissingForNextMaintenanceDto();
            List<Machinery> response = new List<Machinery>();
            List<Region> regions = new List<Region>();
            string? countryName = null;
            List<string>? provincies = new List<string>();
            if (request.CountryId > 0)
            {
                var country = await _countryRepositoryAsync.GetByIdAsync(request.CountryId);
                if (country != null)
                {
                    countryName = country.Name;
                    regions = await _regionRepositoryAsync.ListAsync(new GetRegionByIdSpecification(request.CountryId, request.RegionId, request.DepartmentId));
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
            else
            {
                regions = await _regionRepositoryAsync.ListAsync(new GetRegionByIdSpecification(null, null, null));
            }

            var machineries = await _repositoryAsync.ListAsync(new GetMachineriesSystemRunningSpecification(request.SubcategoryId, countryName, request.BrandId, request.Status));
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
                response = response.Where(a => a.MachineyRootcloudHistoricals.Count > 0 && a.ActiveMaintenance).ToList();
                result.TotalMachineries = response.Count;
            }
            else
            {
                response = machineries.Where(a => a.MachineyRootcloudHistoricals.Count > 0 && a.ActiveMaintenance).ToList();
                result.TotalMachineries = response.Count;
            }

            List<MachineryMissingMaintenanceDto> machineriesInfo = new List<MachineryMissingMaintenanceDto>();
            if (response.Count > 0)
            {
                foreach (var item in response)
                {
                    if (item.MachineyRootcloudHistoricals.Count > 0)
                    {
                        var checkHourMeter = item.MachineyRootcloudHistoricals.LastOrDefault(s => s.MissingForNextMaintenance < request.Hour);
                        if (checkHourMeter != null)
                        {
                            var machinery = new MachineryMissingMaintenanceDto();
                            machinery.SerialNum = item.SerialNum;
                            machinery.Customer = item.Customer;
                            machinery.Hourmeter = checkHourMeter.Hourmeter;
                            machinery.MissingForNextMaintenance = checkHourMeter.MissingForNextMaintenance;
                            machinery.LateHours = checkHourMeter.LateHours;
                            machinery.TimestampLocal = checkHourMeter.TimestampLocal;
                            machinery.NextMaintenance = checkHourMeter.NextMaintenance;
                            machinery.SystemRunning = checkHourMeter.SystemRunning;
                            machinery.CreationDate = checkHourMeter.CreationDate;
                            machinery.Country = item.Country;
                            machinery.Province = item.Province;
                            machinery.Model = item.MachineTypeName;
                            foreach (var region in regions)
                            {
                                foreach (var department in region.Departments)
                                {
                                    if (department.Name == item.Province)
                                    {
                                        machinery.Region = region.Name;
                                    }
                                    else
                                    {
                                        machinery.Region = "Otro";
                                    }
                                }
                            }
                            result.TotalMissing++;
                            machineriesInfo.Add(machinery);
                        }
                    }
                }
                result.TotalNotMissing = result.TotalMachineries - result.TotalMissing;
                result.Machineries = machineriesInfo.OrderBy(o => o.MissingForNextMaintenance).ToList();
            }

            return new Response<MissingForNextMaintenanceDto>(result);
        }
    }
}
