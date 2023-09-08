using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMART.ERP.Application.Features.MachineryFeature.Commands.CreateRootcloudHistoricalCommand;
using SMART.ERP.Application.Features.MachineryFeature.Commands.UpdateRootcloudHistoricalCommand;
using SMART.ERP.Application.Features.MachineryFeature.Queries;
using SMART.ERP.Application.Parameters;
using SMART.ERP.Application.Features.MachineryFeature.Commands.CreateFailureReportCommand;
using SMART.ERP.Application.Features.MachineryFeature.Commands.CreateHourmeterCommand;
using SMART.ERP.Application.Features.MachineryFeature.Commands.CreateMachineryByCsvCommand;
using SMART.ERP.Application.Features.MachineryFeature.Commands.CreateMachineryCommand;
using SMART.ERP.Application.Features.MachineryFeature.Commands.CreateMaintenanceCommand;
using SMART.ERP.Application.Features.MachineryFeature.Commands.UpdateMachineryCommand;
using SMART.ERP.Application.Services.Rootcloud;

namespace SMART.ERP.API.Controllers.v1
{
    [ApiVersion("1.0")]
    [Authorize]
    public class MachineryController : BaseApiController
    {
        private readonly IRootcloudHistoricalService _rootcloudHistoricalService;
        private readonly IRootcloudMachineryService _rootcloudMachineryService;
        private readonly IRootcloudSessionService _rootcloudSessionService;

        public MachineryController(IRootcloudHistoricalService rootcloudHistoricalService,
            IRootcloudMachineryService rootcloudMachineryService, IRootcloudSessionService rootcloudSessionService)
        {
            _rootcloudHistoricalService = rootcloudHistoricalService;
            _rootcloudMachineryService = rootcloudMachineryService;
            _rootcloudSessionService = rootcloudSessionService;
        }

        [HttpGet("GetAllMachines")]
        public async Task<IActionResult> GetAllHistorical([FromQuery] RequestRootcloudParameter requestRootcloud)
        {
            return Ok(await Mediator.Send(new GetAllMachineriesQuery
            {
                All = requestRootcloud.All,
                PageNumber = requestRootcloud.PageNumber,
                PageSize = requestRootcloud.PageSize,
                Parameter = requestRootcloud.Parameter,
                Date = DateTime.Parse(requestRootcloud.Date),
                Column = requestRootcloud.Column,
                Order = requestRootcloud.Order
            }));
        }

        [HttpGet("RegisterMachineries")]
        public async Task<IActionResult> RegisterMachineries()
        {
            return Ok(await _rootcloudMachineryService.GetAllMachineries());
        }

        [HttpGet("GetAllFailureReports")]
        public async Task<IActionResult> GetAllFailureReports([FromQuery] RequestRootcloudParameter requestRootcloud)
        {
            return Ok(await Mediator.Send(new GetAllFailureReportsQuery
            {
                All = requestRootcloud.All,
                PageNumber = requestRootcloud.PageNumber,
                PageSize = requestRootcloud.PageSize,
                Parameter = requestRootcloud.Parameter,
                Column = requestRootcloud.Column,
                Order = requestRootcloud.Order
            }));
        }

        [HttpGet("GetAllMaintenances")]
        public async Task<IActionResult> GetAllMaintenances([FromQuery] RequestRootcloudParameter requestRootcloud)
        {
            return Ok(await Mediator.Send(new GetAllMaintenancesQuery
            {
                All = requestRootcloud.All,
                PageNumber = requestRootcloud.PageNumber,
                PageSize = requestRootcloud.PageSize,
                Parameter = requestRootcloud.Parameter,
                Column = requestRootcloud.Column,
                Order = requestRootcloud.Order
            }));
        }


        [HttpGet("GetWorkingHistorical")]
        public async Task<IActionResult> GetWorkingHistorical([FromQuery] WorkingHistoricalParameter workingHistorical)
        {
            var session = await _rootcloudSessionService.CheckAndUpdateSession();
            if (!session.IsActive)
                return Unauthorized("La sesion con los servicios de rootcloud expiro!! vuelva a intentarlo");
            var response = await _rootcloudHistoricalService.HistoricalWorkingConditions(workingHistorical.BaseInfoId, workingHistorical.DeviceModelId,
            workingHistorical.StartTime, workingHistorical.EndTime, workingHistorical.PageIndex, workingHistorical.PageSize, session);

            return Ok(response);
        }

        [HttpGet("GetAllMachineriesByProvince/{subcategoryId}")]
        public async Task<IActionResult> GetAllMachineriesByProvince(int subcategoryId, [FromQuery] int? countryId, [FromQuery] int? regionId, [FromQuery] int? departmentId, [FromQuery] int? brandId, [FromQuery] string? status)
        {
            return Ok(await Mediator.Send(new GetAllMachineriesByProvinceQuery
            {
                SubcategoryId = subcategoryId,
                CountryId = countryId,
                RegionId = regionId,
                DepartmentId = departmentId,
                BrandId = brandId,
                Status = status
            }));
        }

        [HttpGet("GetAllByInternalMaintenance/{subcategoryId}")]
        public async Task<IActionResult> GetAllByInternalMaintenance(int subcategoryId, [FromQuery] int? countryId, [FromQuery] int? regionId, [FromQuery] int? departmentId, [FromQuery] int? brandId, [FromQuery] string? status)
        {
            return Ok(await Mediator.Send(new GetAllByInternalMaintenanceQuery
            {
                SubcategoryId = subcategoryId,
                CountryId = countryId,
                RegionId = regionId,
                DepartmentId = departmentId,
                BrandId = brandId,
                Status = status
            }));
        }

        [HttpGet("GetAllRootcloudHistorical")]
        public async Task<IActionResult> GetAllRootcloudHistorical()
        {
            return Ok(await Mediator.Send(new CreateRootcloudHistoricalCommand()));
        }

        [HttpGet("GetAllMachineriesByStatus")]
        public async Task<IActionResult> GetAllMachineriesByStatus([FromQuery] int subcategoryId, [FromQuery] int? countryId, [FromQuery] int? regionId, [FromQuery] int? departmentId, [FromQuery] int? brandId, [FromQuery] string? status)
        {
            return Ok(await Mediator.Send(new GetMachineriesByStatusQuery
            {
                SubcategoryId = subcategoryId,
                CountryId = countryId,
                RegionId = regionId,
                DepartmentId = departmentId,
                BrandId = brandId,
                Status = status
            }));
        }

        [HttpGet("GetAllMachineriesByFailure")]
        public async Task<IActionResult> GetAllMachineriesByFailure([FromQuery] int subcategoryId, [FromQuery] int? countryId, [FromQuery] int? regionId, [FromQuery] int? departmentId, [FromQuery] int? brandId, [FromQuery] string? status)
        {
            return Ok(await Mediator.Send(new GetAllMachineriesByFailureQuery
            {
                SubcategoryId = subcategoryId,
                CountryId = countryId,
                RegionId = regionId,
                DepartmentId = departmentId,
                BrandId = brandId,
                Status = status
            }));
        }

        [HttpGet("GetMachineriesBySystemRunning/{subcategoryId}")]
        public async Task<IActionResult> GetMachineriesBySystemRunning(int subcategoryId, [FromQuery] int? countryId, [FromQuery] int? regionId, [FromQuery] int? departmentId, [FromQuery] int? brandId, [FromQuery] string? status)
        {
            return Ok(await Mediator.Send(new GetMachineryBySystemOperativeQuery
            {
                SubcategoryId = subcategoryId,
                CountryId = countryId,
                RegionId = regionId,
                DepartmentId = departmentId,
                BrandId = brandId,
                Status = status
            }));
        }

        [HttpGet("GetMachineryByMissingForNextMaintenance")]
        public async Task<IActionResult> GetMachineryByMissingForNextMaintenance([FromQuery] int hour, [FromQuery] int subcategoryId, [FromQuery] int? countryId, [FromQuery] int? regionId, [FromQuery] int? departmentId, [FromQuery] int? brandId, [FromQuery] string? status)
        {
            if (hour == 0)
                hour = 70;
            return Ok(await Mediator.Send(new GetMachineryByMissingForNextMaintenanceQuery()
            {
                Hour = hour,
                SubcategoryId = subcategoryId,
                CountryId = countryId,
                RegionId = regionId,
                DepartmentId = departmentId,
                BrandId = brandId,
                Status = status
            }));
        }

        [HttpPost("CreateFailureReport")]
        public async Task<IActionResult> CreateFailureReport([FromBody] CreateFailureReportCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        [HttpPost("CreateMachineryByCsv")]
        public async Task<IActionResult> CreateMachineryByCsv([FromForm] CreateMachineryByCsvCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        [HttpPost("CreateMachinery")]
        public async Task<IActionResult> CreateMachinery([FromBody] CreateMachineryCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        [HttpPost("CreateMaintenance")]
        public async Task<IActionResult> CreateMaintenance([FromBody] CreateMaintenanceCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        [HttpPost("CreateHourmeter")]
        public async Task<IActionResult> CreateHourmeter([FromBody] CreateHourmeterCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        [HttpPut("UpdateRootcloudHistorical/{historicalId}")]
        public async Task<IActionResult> UpdateRootcloudHistorical(int historicalId)
        {
            return Ok(await Mediator.Send(new UpdateRootcloudHistoricalCommand { Id = historicalId }));
        }

        [HttpPut("UpdateMachinery/{id}")]
        public async Task<IActionResult> UpdateMachinery(int id, [FromBody] UpdateMachineryCommand command)
        {
            if (id != command.Id)
                return BadRequest(new { message = "Ocurrio un problema con el id de este registro" });

            return Ok(await Mediator.Send(command));
        }
    }
}
