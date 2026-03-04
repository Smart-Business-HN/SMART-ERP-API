using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMART.ERP.Application.Features.CartFeature.Commands.AddProductToCartCommand;
using SMART.ERP.Application.Features.CartFeature.Queries.GetCartByIdQuery;
using SMART.ERP.Application.Features.CartFeature.Queries.GetCartsByCustomerIdQuery;

namespace SMART.ERP.API.Controllers.v2
{
    [ApiVersion("2.0")]
    public class CartController : BaseApiController
    {
        [Authorize]
        [HttpPut("AddProduct/{productId}")]
        public async Task<IActionResult> AddProduct([FromRoute] int productId, [FromQuery] int quantity, Guid customerId, Guid? cartId, bool forceNewCart = false)
        {
            return  Ok(await  Mediator.Send( new AddProductToCartCommand
            {
                CartId = cartId,
                ProductId = productId,
                Quantity = quantity,
                CustomerId = customerId,
                ForceNewCart = forceNewCart
            }));
        }
        [Authorize]
        [HttpGet("GetCarts/{customerId}")]
        public async Task<IActionResult> GetCarts([FromRoute] Guid customerId)
        {
            return Ok(await Mediator.Send(new GetCartsByCustomerIdQuery { CustomerId = customerId }));
        }

        [Authorize]
        [HttpGet("GetCartById/{id}")]
        public async Task<IActionResult> GetCartById([FromRoute] Guid id)
        {
            return Ok(await Mediator.Send(new GetCartByIdQuery(){Id = id}));
        }
    }
}
