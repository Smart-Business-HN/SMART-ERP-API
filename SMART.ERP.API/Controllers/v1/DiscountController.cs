using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using SMART.ERP.Application.Features.DiscountFeature.Commands.CreateDiscountCommand;
using SMART.ERP.Application.Features.DiscountFeature.Commands.DeleteDiscountCommand;
using SMART.ERP.Application.Features.DiscountFeature.Commands.UpdateDiscountCommand;
using SMART.ERP.Application.Features.DiscountFeature.Queries;
using SMART.ERP.Application.Parameters;

namespace SMART.ERP.API.Controllers.v1
{
    [ApiVersion("1.0")]
    public class DiscountController : BaseApiController
    {
        [HttpPost("Create")]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateDiscountCommand command)
        {
            return Ok(await Mediator.Send(command));
        }
        [HttpGet("GetAll")]
        [AllowAnonymous]
        [OutputCache(PolicyName = "cache_discounts")]
        public async Task<IActionResult> GetAll([FromQuery] RequestParameter filter)
        {

            return Ok(await Mediator.Send(new GetAllDiscountQuery()
            {
                Parameter = filter.Parameter,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                Order = filter.Order,
                Column = filter.Column,
                All = filter.All
            }));
        }

        [HttpPut("Update/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateDiscountCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest("Ocurrio un error con el id de este registro");
            }
            return Ok(await Mediator.Send(command));
        }
        [HttpDelete("Delete/{id}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Delete(int id)
        {
            return Ok(await Mediator.Send(new DeleteDiscountCommand { Id = id }));
        }
    }
}

