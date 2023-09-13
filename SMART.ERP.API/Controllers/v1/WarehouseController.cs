using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMART.ERP.Application.Features.WarehouseFeature.Commands.CreateWarehouseCommand;
using SMART.ERP.Application.Features.WarehouseFeature.Commands.UpdateWarehouseCommand;
using SMART.ERP.Application.Features.WarehouseFeature.Queries;
using SMART.ERP.Application.Features.WinReasonFeature.Commands.DeleteWinReasonCommand;
using SMART.ERP.Application.Features.WishListFeature.Queries;
using SMART.ERP.Application.Parameters;

namespace SMART.ERP.API.Controllers.v1
{

    public class WarehouseController : BaseApiController
    {
        [HttpPost("Create")]
        [Authorize(Roles = "SuperAdmin, Manager")]
        public async Task<IActionResult> Create([FromBody] CreateWarehouseCommand command)
        {
            return Ok(await Mediator.Send(command));
        }
        [HttpGet("GetAll")]
        [Authorize]
        public async Task<IActionResult> GetAll([FromQuery] RequestParameter filter)
        {
            return Ok(await Mediator.Send(new GetAllWarehouseQuery()
            {
                Parameter = filter.Parameter,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                Order = filter.Order,
                Column = filter.Column,
                All = filter.All
            }));
        }

        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            return Ok(await Mediator.Send(new GetWarehouseByIdQuery { Id = id }));
        }

        [HttpPut("Update/{id}")]
        [Authorize(Roles = "SuperAdmin, Manager")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateWarehouseCommand command)
        {
            if (id != command.Id)
                return BadRequest(new { message = "Ocurrio un problema con el id de este regitro" });
            return Ok(await Mediator.Send(command));
        }

        [HttpDelete("Delete/{id}")]
        [Authorize(Roles = "SuperAdmin, Manager")]
        public async Task<IActionResult> Delete(int id)
        {
            return Ok(await Mediator.Send(new DeleteWinReasonCommand { Id = id }));
        }
       

    }
}
