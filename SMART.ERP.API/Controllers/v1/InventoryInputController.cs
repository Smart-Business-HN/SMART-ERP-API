using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMART.ERP.Application.Features.InventoryInputFeature.Commands.CreateInventoryInputCommand;
using SMART.ERP.Application.Features.InventoryInputFeature.Commands.CreateInventoryInputCommandByPurchaseOrderIdCommand;
using SMART.ERP.Application.Features.InventoryInputFeature.Commands.DeleteInventoryInputCommand;
using SMART.ERP.Application.Features.InventoryInputFeature.Queries;
using SMART.ERP.Application.Parameters;

namespace SMART.ERP.API.Controllers.v1
{
    [ApiVersion("1.0")]
    public class InventoryInputController : BaseApiController
    {
        [HttpPost("Create")]
        [Authorize(Roles = "SuperAdmin, Manager, Warehouse Agent, Admin, Purchasing Agent")]
        public async Task<IActionResult> Create([FromBody] CreateInventoryInputCommand command)
        {
            return Ok(await Mediator.Send(command));
        }
        [HttpPost("CreateByPurchaseOrderId")]
        [Authorize(Roles = "SuperAdmin, Manager, Purchasing Agent, Admin")]
        public async Task<IActionResult> CreateByPurchaseOrderId([FromBody] CreateInventoryInputByPurchaseOrderIdCommand command)
        {
            return Ok(await Mediator.Send(command));
        }
        [HttpGet("GetById/{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(int id)
        {
            return Ok(await Mediator.Send(new GetInventoryInputByIdQuery { Id = id }));
        }

        [HttpGet("GetAll")]
        //[Authorize]
        public async Task<IActionResult> GetAll([FromQuery] RequestParameter filter)
        {
            return Ok(await Mediator.Send(new GetAllInventoryInputQuery()
            {
                Parameter = filter.Parameter,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                Order = filter.Order,
                Column = filter.Column,
                All = filter.All
            }));
        }

        //[HttpPut("Update/{id}")]
        //[Authorize(Roles = "SuperAdmin, Manager, Admin")]
        //public async Task<IActionResult> Update(int id, [FromBody] UpdateInventoryInputTypeCommand command)
        //{
        //    if (id != command.Id)
        //    {
        //        return BadRequest("Ocurrio un error con el id de este registro");
        //    }
        //    return Ok(await Mediator.Send(command));
        //}
       
        [HttpDelete("Delete/{id}")]
        [Authorize(Roles = "SuperAdmin, Manager")]
        public async Task<IActionResult> Delete(int id)
        {
            return Ok(await Mediator.Send(new DeleteInventoryInputCommand { Id = id }));
        }
    }
}
