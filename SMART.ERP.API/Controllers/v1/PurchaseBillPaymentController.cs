using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMART.ERP.Application.Features.PurchaseBillPaymentFeature.Commands.CreatePurchaseBillPaymentCommand;

namespace SMART.ERP.API.Controllers.v1
{
    [ApiVersion("1.0")]
    public class PurchaseBillPaymentController : BaseApiController
    {
        [HttpPost("Create")]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreatePurchaseBillPaymentCommand command)
        {
            return Ok(await Mediator.Send(command));
        }
    }
}
