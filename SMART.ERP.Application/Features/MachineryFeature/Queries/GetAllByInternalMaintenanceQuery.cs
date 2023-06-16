using MediatR;
using SMART.ERP.Application.DTOs.Rootcloud;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.MachinerySpecification;
using SMART.ERP.Application.Specifications.RegionSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.MachineryFeature.Queries
{
    public class GetAllByInternalMaintenanceQuery : IRequest<Response<List<InternalMaintenanceDto>>>
    {
        public int SubcategoryId { get; set; }
        public int? CountryId { get; set; }
        public int? RegionId { get; set; }
        public int? DepartmentId { get; set; }
        public int? BrandId { get; set; }
        public string? Status { get; set; }
    }

    public class GetAllByInternalMaintenanceHandler : IRequestHandler<GetAllByInternalMaintenanceQuery, Response<List<InternalMaintenanceDto>>>
    {
        private readonly IRepositoryAsync<Machinery> _repositoryAsync;
        private readonly IRepositoryAsync<Region> _regionRepositoryAsync;
        private readonly IRepositoryAsync<Country> _countryRepositoryAsync;

        public GetAllByInternalMaintenanceHandler(IRepositoryAsync<Machinery> repositoryAsync,
            IRepositoryAsync<Region> regionRepositoryAsync, IRepositoryAsync<Country> countryRepositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
            _regionRepositoryAsync = regionRepositoryAsync;
            _countryRepositoryAsync = countryRepositoryAsync;
        }

        public async Task<Response<List<InternalMaintenanceDto>>> Handle(GetAllByInternalMaintenanceQuery request, CancellationToken cancellationToken)
        {
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
            }
            else
            {
                response = machineries.ToList();
            }

            var group = response.GroupBy(a => a.ActiveMaintenance).Select(a => new InternalMaintenanceDto
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
                    Province = sm.Province,
                    LateHours = sm.MachineyRootcloudHistoricals.First().LateHours,
                    Model = sm.MachineTypeName,
                }).OrderBy(o => o.MissingForNextMaintenance).ToList()
            }).ToList();

            return new Response<List<InternalMaintenanceDto>>(group);
        }
    }
}
