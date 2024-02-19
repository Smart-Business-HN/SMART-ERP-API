using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMART.ERP.Application.Features.PurchaseBillFeature.Commands.CreatePurchaseBillCommand;
using SMART.ERP.Application.Features.PurchaseBillFeature.Commands.CreatePurchaseBillFromPurchaseOrderDetailPageCommand;
using SMART.ERP.Application.Features.PurchaseBillFeature.Commands.UpdatePurchaseBillCommand;
using SMART.ERP.Application.Features.PurchaseBillFeature.Queries;
using SMART.ERP.Application.Parameters;

namespace SMART.ERP.API.Controllers.v1
{
    [ApiVersion("1.0")]
    public class PurchaseBillController : BaseApiController
    {
        [HttpPost("CreatePurchaseBillFromPurchaseOrderDetailPage")]
        [Authorize(Roles = "SuperAdmin, Admin, Manager, SalesAdvisor")]
        public async Task<IActionResult> CreatePurchaseBillFromPurchaseOrderDetailPage([FromBody] CreatePurchaseBillFromPurchaseOrderDetailPageCommand command)
        {
            return Ok(await Mediator.Send(command));
        }
        [HttpPost("Create")]
        [Authorize(Roles = "SuperAdmin, Admin, Manager, SalesAdvisor")]
        public async Task<IActionResult> Create([FromBody] CreatePurchaseBillCommand command)
        {
            return Ok(await Mediator.Send(command));
        }
        [HttpGet("GetAll")]
        [Authorize(Roles = "SuperAdmin, Admin, Manager, SalesAdvisor")]
        public async Task<IActionResult> GetAll([FromQuery] RequestParameter filter)
        {
            return Ok(await Mediator.Send(new GetAllPurchaseBillQuery
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
        [Authorize(Roles = "SuperAdmin, Admin, Manager, SalesAdvisor")]
        public async Task<IActionResult> GetById(int id)
        {
            return Ok(await Mediator.Send(new GetPurchaseBillByIdQuery { Id = id }));
        }
        [HttpPut("Update/{id}")]
        [Authorize(Roles = "SuperAdmin, Admin, Manager, SalesAdvisor")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdatePurchaseBillCommand command)
        {
            if (id != command.Id)
                return BadRequest(new
                {
                    message = "Ocurrio un problema con el id de este regitro"
                });
            return Ok(await Mediator.Send(command));
        }
    }
}
