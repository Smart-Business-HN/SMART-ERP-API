using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMART.ERP.Application.Features.SaleOrderFeature.Commands.DeleteSaleOrderCommand;
using SMART.ERP.Application.Features.SaleOrderFeature.Queries;
using SMART.ERP.Application.Parameters;
using SMART.ERP.API.Controllers;
using SMART.ERP.Application.Features.SaleOrderFeature.Commands.CreateSaleOrderCommand;

namespace SMART.ERP.API.Controllers.v1
{
    [ApiVersion("1.0")]
    [Authorize]
    public class SaleOrderController : BaseApiController
    {
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll([FromQuery] RequestParameter filter)
        {
            return Ok(await Mediator.Send(new GetAllSaleOrdersQuery()
            {
                Parameter = filter.Parameter,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                Order = filter.Order,
                Column = filter.Column,
            }));
        }

        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            return Ok(await Mediator.Send(new GetSaleOrderByIdQuery { Id = id }));
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] CreateSaleOrderCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            return Ok(await Mediator.Send(new DeleteSaleOrderCommand { Id = id }));
        }
    }
}
