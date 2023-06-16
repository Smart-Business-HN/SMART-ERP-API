using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMART.ERP.Application.Features.WishListFeature.Commands.DeleteWishListCommand;
using SMART.ERP.Application.Features.WishListFeature.Queries;
using SMART.ERP.Application.Parameters;
using SMART.ERP.API.Controllers;
using SMART.ERP.Application.Features.WishListFeature.Commands.CreateWishListCommand;
using SMART.ERP.Application.Features.WishListFeature.Commands.UpdateWishListCommand;

namespace SMART.ERP.API.Controllers.v1
{

    [ApiVersion("1.0")]
    [Authorize]
    public class WishListController : BaseApiController
    {
        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] CreateWishListCommand command)
        {
            return Ok(await Mediator.Send(command));
        }
        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            return Ok(await Mediator.Send(new GetWishListByIdQuery { Id = id }));
        }

        [HttpGet("GetByCustomerId/{id}")]
        public async Task<IActionResult> GetByCustomerId(Guid id)
        {
            return Ok(await Mediator.Send(new GetWishListByCustomerIdQuery { CustomerId = id }));
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll([FromQuery] RequestParameter filter)
        {
            return Ok(await Mediator.Send(new GetAllWishListsQuery()
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
        public async Task<IActionResult> Update(int id, [FromBody] UpdateWishListCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest("Ocurrio un error con el id de este registro");
            }
            return Ok(await Mediator.Send(command));
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            return Ok(await Mediator.Send(new DeleteWishListCommand { Id = id }));
        }
    }
}
