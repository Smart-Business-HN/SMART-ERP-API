using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMART.ERP.Application.Features.CartFeature.Commands.AdminAddProductToCartCommand;
using SMART.ERP.Application.Features.CartFeature.Commands.AdminDeleteCartCommand;
using SMART.ERP.Application.Features.CartFeature.Commands.AdminRemoveCartItemCommand;
using SMART.ERP.Application.Features.CartFeature.Commands.AdminUpdateCartItemCommand;
using SMART.ERP.Application.Features.CartFeature.Commands.AdminUpdateCartStatusCommand;
using SMART.ERP.Application.Features.CartFeature.Queries.AdminGetCartsByEcommerceUserIdQuery;

namespace SMART.ERP.API.Controllers.v1
{
    [ApiVersion("1.0")]
    public class CartController : BaseApiController
    {
        [HttpGet("GetCartsByUser/{ecommerceUserId}")]
        [Authorize(Roles = "SuperAdmin, Manager")]
        public async Task<IActionResult> GetCartsByUser(Guid ecommerceUserId, [FromQuery] bool? isActive)
        {
            return Ok(await Mediator.Send(new AdminGetCartsByEcommerceUserIdQuery
            {
                EcommerceUserId = ecommerceUserId,
                IsActive = isActive
            }));
        }

        [HttpPost("AddProduct")]
        [Authorize(Roles = "SuperAdmin, Manager")]
        public async Task<IActionResult> AddProduct([FromBody] AdminAddProductToCartCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        [HttpPut("UpdateCartItem")]
        [Authorize(Roles = "SuperAdmin, Manager")]
        public async Task<IActionResult> UpdateCartItem([FromBody] AdminUpdateCartItemCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        [HttpDelete("RemoveCartItem/{cartItemId}")]
        [Authorize(Roles = "SuperAdmin, Manager")]
        public async Task<IActionResult> RemoveCartItem(int cartItemId)
        {
            return Ok(await Mediator.Send(new AdminRemoveCartItemCommand { CartItemId = cartItemId }));
        }

        [HttpDelete("DeleteCart/{cartId}")]
        [Authorize(Roles = "SuperAdmin, Manager")]
        public async Task<IActionResult> DeleteCart(Guid cartId)
        {
            return Ok(await Mediator.Send(new AdminDeleteCartCommand { CartId = cartId }));
        }

        [HttpPut("UpdateCartStatus")]
        [Authorize(Roles = "SuperAdmin, Manager")]
        public async Task<IActionResult> UpdateCartStatus([FromBody] AdminUpdateCartStatusCommand command)
        {
            return Ok(await Mediator.Send(command));
        }
    }
}
