using MediatR;
using SMART.ERP.Application.DTOs.Rootcloud;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.MachinerySpecification;
using SMART.ERP.Application.Specifications.RegionSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.MachineryFeature.Queries
{
    public class GetMachineryBySystemOperativeQuery : IRequest<Response<MachinerySystemRunningDto>>
    {
        public int SubcategoryId { get; set; }
        public int? CountryId { get; set; }
        public int? RegionId { get; set; }
        public int? DepartmentId { get; set; }
        public int? BrandId { get; set; }
        public string? Status { get; set; }
    }

    public class GetMachineryBySystemOperativeQueryHandler : IRequestHandler<GetMachineryBySystemOperativeQuery, Response<MachinerySystemRunningDto>>
    {
        private readonly IRepositoryAsync<Machinery> _repositoryAsync;
        private readonly IRepositoryAsync<Region> _regionRepositoryAsync;
        private readonly IRepositoryAsync<Country> _countryRepositoryAsync;

        public GetMachineryBySystemOperativeQueryHandler(IRepositoryAsync<Machinery> repositoryAsync,
            IRepositoryAsync<Region> regionRepositoryAsync, IRepositoryAsync<Country> countryRepositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
            _regionRepositoryAsync = regionRepositoryAsync;
            _countryRepositoryAsync = countryRepositoryAsync;
        }
        public async Task<Response<MachinerySystemRunningDto>> Handle(GetMachineryBySystemOperativeQuery request, CancellationToken cancellationToken)
        {
            var result = new MachinerySystemRunningDto();
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
                response = response.Where(a => a.MachineyRootcloudHistoricals.Count > 0 && a.IsRootcloudActive).ToList();
                result.TotalMachineries = response.Count;
            }
            else
            {
                response = machineries.Where(a => a.MachineyRootcloudHistoricals.Count > 0 && a.IsRootcloudActive).ToList();
                result.TotalMachineries = response.Count;
            }

            if (response.Count > 0)
            {
                List<MachineryMissingMaintenanceDto> list = new List<MachineryMissingMaintenanceDto>();
                foreach (var item in response)
                {
                    var systemRunningOrNot = new MachineryMissingMaintenanceDto();
                    if (item.MachineyRootcloudHistoricals?.Count > 0)
                    {
                        if (item.MachineyRootcloudHistoricals.First().TimestampLocal != "N/A")
                        {
                            double diffOfDates = (DateTime.Now.Date - Convert.ToDateTime(item.MachineyRootcloudHistoricals.First().TimestampLocal).Date).TotalDays;
                            if (diffOfDates < 5 && item.MachineyRootcloudHistoricals.First().Hourmeter > 0)
                            {
                                systemRunningOrNot.Hourmeter = item.MachineyRootcloudHistoricals.First().Hourmeter;
                                systemRunningOrNot.MissingForNextMaintenance = item.MachineyRootcloudHistoricals.First().MissingForNextMaintenance;
                                systemRunningOrNot.NextMaintenance = item.MachineyRootcloudHistoricals.First().NextMaintenance;
                                systemRunningOrNot.SerialNum = item.SerialNum;
                                systemRunningOrNot.LateHours = item.MachineyRootcloudHistoricals.First().LateHours;
                                systemRunningOrNot.Customer = item.Customer;
                                systemRunningOrNot.TimestampLocal = item.MachineyRootcloudHistoricals.First().TimestampLocal;
                                systemRunningOrNot.CreationDate = item.MachineyRootcloudHistoricals.First().CreationDate;
                                systemRunningOrNot.SystemRunning = true;
                                systemRunningOrNot.Country = item.Country;
                                systemRunningOrNot.Province = item.Province;
                                systemRunningOrNot.Model = item.MachineTypeName;
                                list.Add(systemRunningOrNot);
                                result.SystemRunning++;
                            }
                            else
                            {
                                if (diffOfDates > 5 || item.MachineyRootcloudHistoricals.First().Hourmeter == 0)
                                {
                                    systemRunningOrNot.Hourmeter = item.MachineyRootcloudHistoricals.First().Hourmeter;
                                    systemRunningOrNot.MissingForNextMaintenance = item.MachineyRootcloudHistoricals.First().MissingForNextMaintenance;
                                    systemRunningOrNot.NextMaintenance = item.MachineyRootcloudHistoricals.First().NextMaintenance;
                                    systemRunningOrNot.SerialNum = item.SerialNum;
                                    systemRunningOrNot.LateHours = item.MachineyRootcloudHistoricals.First().LateHours;
                                    systemRunningOrNot.Customer = item.Customer;
                                    systemRunningOrNot.TimestampLocal = item.MachineyRootcloudHistoricals.First().TimestampLocal;
                                    systemRunningOrNot.CreationDate = item.MachineyRootcloudHistoricals.First().CreationDate;
                                    systemRunningOrNot.SystemRunning = false;
                                    systemRunningOrNot.Country = item.Country;
                                    systemRunningOrNot.Province = item.Province;
                                    systemRunningOrNot.Model = item.MachineTypeName;
                                    list.Add(systemRunningOrNot);
                                    result.SystemNotRunning++;
                                }
                            }
                        }
                    }
                }
                result.Machineries = list.OrderBy(o => o.MissingForNextMaintenance).ToList();
            }
            return new Response<MachinerySystemRunningDto>(result);

        }
    }
}
