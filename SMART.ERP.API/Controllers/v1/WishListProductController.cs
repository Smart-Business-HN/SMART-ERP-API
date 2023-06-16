using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMART.ERP.Application.Features.WishListProductFeature.Commands.DeleteWishListProductCommand;
using SMART.ERP.Application.Features.WishListProductFeature.Queries;
using SMART.ERP.API.Controllers;
using SMART.ERP.Application.Features.WishListProductFeature.Commands.CreateWishListProductCommand;
using SMART.ERP.Application.Features.WishListProductFeature.Commands.UpdateWishListProductCommand;

namespace SMART.ERP.API.Controllers.v1
{
    [ApiVersion("1.0")]
    [Authorize]
    public class WishListProductController : BaseApiController
    {
        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] CreateWishListProductCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            return Ok(await Mediator.Send(new GetWishListProductByIdQuery { Id = id }));
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await Mediator.Send(new GetAllWishListProductQuery()));
        }

        [HttpPut("Update/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateWishListProductCommand command)
        {
            if (id != command.Id)
                return BadRequest(new { message = "Ocurrio un problema con el id de este registro" });
            return Ok(await Mediator.Send(command));
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            return Ok(await Mediator.Send(new DeleteWishListProductCommand { Id = id }));
        }
    }
}
