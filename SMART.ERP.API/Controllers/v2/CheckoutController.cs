using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMART.ERP.Application.Features.CheckoutFeature.Commands.RequestPaymentLinkCommand;
using SMART.ERP.Application.Features.CheckoutFeature.Commands.SubmitTransferReceiptCommand;

namespace SMART.ERP.API.Controllers.v2
{
    [ApiVersion("2.0")]
    [Authorize]
    public class CheckoutController : BaseApiController
    {
        [HttpPost("RequestPaymentLink")]
        public async Task<IActionResult> RequestPaymentLink([FromBody] RequestPaymentLinkCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        [HttpPost("SubmitTransferReceipt")]
        public async Task<IActionResult> SubmitTransferReceipt([FromForm] SubmitTransferReceiptCommand command)
        {
            return Ok(await Mediator.Send(command));
        }
    }
}
