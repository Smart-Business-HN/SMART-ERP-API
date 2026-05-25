using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMART.ERP.Application.Features.WarehouseTransferFeature.Commands.CancelWarehouseTransferCommand;
using SMART.ERP.Application.Features.WarehouseTransferFeature.Commands.CompleteWarehouseTransferCommand;
using SMART.ERP.Application.Features.WarehouseTransferFeature.Commands.CreateWarehouseTransferCommand;
using SMART.ERP.Application.Features.WarehouseTransferFeature.Commands.ReceiveWarehouseTransferCommand;
using SMART.ERP.Application.Features.WarehouseTransferFeature.Commands.SendWarehouseTransferCommand;
using SMART.ERP.Application.Features.WarehouseTransferFeature.Queries;
using SMART.ERP.Application.Parameters;
using SMART.ERP.Domain.Enums;

namespace SMART.ERP.API.Controllers.v1
{
    [ApiVersion("1.0")]
    public class WarehouseTransferController : BaseApiController
    {
        [HttpPost("Create")]
        [Authorize(Roles = "SuperAdmin, Manager, Warehouse Agent, Admin")]
        public async Task<IActionResult> Create([FromBody] CreateWarehouseTransferCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        [HttpGet("GetById/{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(int id)
        {
            return Ok(await Mediator.Send(new GetWarehouseTransferByIdQuery { Id = id }));
        }

        [HttpGet("GetAll")]
        [Authorize]
        public async Task<IActionResult> GetAll([FromQuery] RequestParameter filter,
            [FromQuery] WarehouseTransferStatus? status, [FromQuery] int? originWarehouseId, [FromQuery] int? destinationWarehouseId)
        {
            return Ok(await Mediator.Send(new GetAllWarehouseTransferQuery
            {
                Parameter = filter.Parameter,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                All = filter.All,
                Status = status,
                OriginWarehouseId = originWarehouseId,
                DestinationWarehouseId = destinationWarehouseId
            }));
        }

        [HttpPatch("Send/{id}")]
        [Authorize(Roles = "SuperAdmin, Manager, Warehouse Agent, Admin")]
        public async Task<IActionResult> Send(int id)
        {
            return Ok(await Mediator.Send(new SendWarehouseTransferCommand { Id = id }));
        }

        [HttpPatch("Receive/{id}")]
        [Authorize(Roles = "SuperAdmin, Manager, Warehouse Agent, Admin")]
        public async Task<IActionResult> Receive(int id)
        {
            return Ok(await Mediator.Send(new ReceiveWarehouseTransferCommand { Id = id }));
        }

        [HttpPatch("Complete/{id}")]
        [Authorize(Roles = "SuperAdmin, Manager, Warehouse Agent, Admin")]
        public async Task<IActionResult> Complete(int id)
        {
            return Ok(await Mediator.Send(new CompleteWarehouseTransferCommand { Id = id }));
        }

        [HttpPatch("Cancel/{id}")]
        [Authorize(Roles = "SuperAdmin, Manager")]
        public async Task<IActionResult> Cancel(int id, [FromBody] CancelWarehouseTransferCommand command)
        {
            command.Id = id;
            return Ok(await Mediator.Send(command));
        }
    }
}
