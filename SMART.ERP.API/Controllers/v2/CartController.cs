using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMART.ERP.Application.Features.CartFeature.Commands.AddProductToCartCommand;

namespace SMART.ERP.API.Controllers.v2
{
    [ApiVersion("2.0")]
    public class CartController : BaseApiController
    {
        [Authorize]
        [HttpPut("AddProduct/{productId}")]
        public async Task<IActionResult> AddProduct([FromRoute] int productId, [FromQuery] int quantity, Guid customerId, Guid? cartId)
        {
            return  Ok(await  Mediator.Send( new AddProductToCartCommand
            {
                CartId = cartId,
                ProductId = productId,
                Quantity = quantity,
                CustomerId = customerId
            }));
        }
    }
}
