using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMART.ERP.Application.Features.InventoryExitFeature.Commands.CancelInventoryExitCommand;
using SMART.ERP.Application.Features.InventoryExitFeature.Commands.ConfirmInventoryExitCommand;
using SMART.ERP.Application.Features.InventoryExitFeature.Commands.CreateInventoryExitCommand;
using SMART.ERP.Application.Features.InventoryExitFeature.Commands.DeleteInventoryExitCommand;
using SMART.ERP.Application.Features.InventoryExitFeature.Commands.UpdateInventoryExitCommand;
using SMART.ERP.Application.Features.InventoryExitFeature.Queries;
using SMART.ERP.Application.Parameters;
using SMART.ERP.Domain.Enums;

namespace SMART.ERP.API.Controllers.v1
{
    [ApiVersion("1.0")]
    public class InventoryExitController : BaseApiController
    {
        [HttpPost("Create")]
        [Authorize(Roles = "SuperAdmin, Manager, Warehouse Agent, Admin")]
        public async Task<IActionResult> Create([FromBody] CreateInventoryExitCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        [HttpGet("GetById/{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(int id)
        {
            return Ok(await Mediator.Send(new GetInventoryExitByIdQuery { Id = id }));
        }

        [HttpGet("GetAll")]
        [Authorize]
        public async Task<IActionResult> GetAll([FromQuery] RequestParameter filter,
            [FromQuery] InventoryExitReason? reason, [FromQuery] InventoryExitStatus? status, [FromQuery] int? warehouseId,
            [FromQuery] int? projectId)
        {
            return Ok(await Mediator.Send(new GetAllInventoryExitQuery
            {
                Parameter = filter.Parameter,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                All = filter.All,
                Reason = reason,
                Status = status,
                WarehouseId = warehouseId,
                ProjectId = projectId
            }));
        }

        [HttpPut("Update/{id}")]
        [Authorize(Roles = "SuperAdmin, Manager, Warehouse Agent, Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateInventoryExitCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest("Ocurrió un error con el id de este registro");
            }
            return Ok(await Mediator.Send(command));
        }

        [HttpDelete("Delete/{id}")]
        [Authorize(Roles = "SuperAdmin, Manager")]
        public async Task<IActionResult> Delete(int id)
        {
            return Ok(await Mediator.Send(new DeleteInventoryExitCommand { Id = id }));
        }

        [HttpPatch("Confirm/{id}")]
        [Authorize(Roles = "SuperAdmin, Manager, Warehouse Agent, Admin")]
        public async Task<IActionResult> Confirm(int id)
        {
            return Ok(await Mediator.Send(new ConfirmInventoryExitCommand { Id = id }));
        }

        [HttpPatch("Cancel/{id}")]
        [Authorize(Roles = "SuperAdmin, Manager")]
        public async Task<IActionResult> Cancel(int id, [FromBody] CancelInventoryExitCommand command)
        {
            command.Id = id;
            return Ok(await Mediator.Send(command));
        }
    }
}
