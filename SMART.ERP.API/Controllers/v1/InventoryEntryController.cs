using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMART.ERP.Application.Features.InventoryEntryFeature.Commands.CancelInventoryEntryCommand;
using SMART.ERP.Application.Features.InventoryEntryFeature.Commands.ConfirmInventoryEntryCommand;
using SMART.ERP.Application.Features.InventoryEntryFeature.Commands.CreateInventoryEntryCommand;
using SMART.ERP.Application.Features.InventoryEntryFeature.Commands.CreateInventoryEntryByPurchaseOrderIdCommand;
using SMART.ERP.Application.Features.InventoryEntryFeature.Commands.DeleteInventoryEntryCommand;
using SMART.ERP.Application.Features.InventoryEntryFeature.Commands.UpdateInventoryEntryCommand;
using SMART.ERP.Application.Features.InventoryEntryFeature.Queries;
using SMART.ERP.Application.Parameters;
using SMART.ERP.Domain.Enums;

namespace SMART.ERP.API.Controllers.v1
{
    [ApiVersion("1.0")]
    public class InventoryEntryController : BaseApiController
    {
        [HttpPost("Create")]
        [Authorize(Roles = "SuperAdmin, Manager, Warehouse Agent, Admin, Purchasing Agent")]
        public async Task<IActionResult> Create([FromBody] CreateInventoryEntryCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        [HttpPost("CreateByPurchaseOrder")]
        [Authorize(Roles = "SuperAdmin, Manager, Purchasing Agent, Admin")]
        public async Task<IActionResult> CreateByPurchaseOrder([FromBody] CreateInventoryEntryByPurchaseOrderIdCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        [HttpGet("GetById/{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(int id)
        {
            return Ok(await Mediator.Send(new GetInventoryEntryByIdQuery { Id = id }));
        }

        [HttpGet("GetAll")]
        [Authorize]
        public async Task<IActionResult> GetAll([FromQuery] RequestParameter filter,
            [FromQuery] InventoryEntryType? entryType, [FromQuery] InventoryEntryStatus? status, [FromQuery] int? warehouseId)
        {
            return Ok(await Mediator.Send(new GetAllInventoryEntryQuery
            {
                Parameter = filter.Parameter,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                All = filter.All,
                EntryType = entryType,
                Status = status,
                WarehouseId = warehouseId
            }));
        }

        [HttpPut("Update/{id}")]
        [Authorize(Roles = "SuperAdmin, Manager, Warehouse Agent, Admin, Purchasing Agent")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateInventoryEntryCommand command)
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
            return Ok(await Mediator.Send(new DeleteInventoryEntryCommand { Id = id }));
        }

        [HttpPatch("Confirm/{id}")]
        [Authorize(Roles = "SuperAdmin, Manager, Warehouse Agent, Admin, Purchasing Agent")]
        public async Task<IActionResult> Confirm(int id)
        {
            return Ok(await Mediator.Send(new ConfirmInventoryEntryCommand { Id = id }));
        }

        [HttpPatch("Cancel/{id}")]
        [Authorize(Roles = "SuperAdmin, Manager")]
        public async Task<IActionResult> Cancel(int id, [FromBody] CancelInventoryEntryCommand command)
        {
            command.Id = id;
            return Ok(await Mediator.Send(command));
        }
    }
}
