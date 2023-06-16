using MediatR;
using SMART.ERP.Application.DTOs.Rootcloud;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.MachinerySpecification;
using SMART.ERP.Application.Specifications.RegionSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.MachineryFeature.Queries
{
    public class GetAllMachineriesByProvinceQuery : IRequest<Response<List<MachineryByProvinceDto>>>
    {
        public int SubcategoryId { get; set; }
        public int? CountryId { get; set; }
        public int? RegionId { get; set; }
        public int? DepartmentId { get; set; }
        public int? BrandId { get; set; }
        public string? Status { get; set; }
    }

    public class GetAllMachineriesByProvinceQueryHandler : IRequestHandler<GetAllMachineriesByProvinceQuery, Response<List<MachineryByProvinceDto>>>
    {
        private readonly IRepositoryAsync<Machinery> _repositoryAsync;
        private readonly IRepositoryAsync<Region> _regionRepositoryAsync;
        private readonly IRepositoryAsync<Country> _countryRepositoryAsync;

        public GetAllMachineriesByProvinceQueryHandler(IRepositoryAsync<Machinery> repositoryAsync,
            IRepositoryAsync<Region> regionRepositoryAsync, IRepositoryAsync<Country> countryRepositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
            _regionRepositoryAsync = regionRepositoryAsync;
            _countryRepositoryAsync = countryRepositoryAsync;
        }

        public async Task<Response<List<MachineryByProvinceDto>>> Handle(GetAllMachineriesByProvinceQuery request, CancellationToken cancellationToken)
        {
            List<Machinery> response = new List<Machinery>();
            string? countryName = null;
            List<Region> regions = new List<Region>();
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
            }
            else
            {
                response = machineries.ToList();
            }

            var group = response.GroupBy(a => new { a.Province, a.Country }).Select(a => new MachineryByProvinceDto
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
                    Province = sm.Province,
                    Region = regions.Select(a => a.Departments).Any()
                    && regions.Where(a => a.Departments.FirstOrDefault(a => a.Name == sm.Province) != null).Any() ?
                    regions.Where(a => a.Departments.FirstOrDefault(a => a.Name == sm.Province) != null).First().Name : "Otro",
                    LateHours = sm.MachineyRootcloudHistoricals.First().LateHours,
                    Model = sm.MachineTypeName
                }).OrderBy(o => o.MissingForNextMaintenance).ToList()
            }).ToList();
            return new Response<List<MachineryByProvinceDto>>(group);
        }
    }
}
