using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMART.ERP.Application.Features.PurchaseBillFeature.Commands.CreatePurchaseBillFromPurchaseOrderDetailPageCommand;

namespace SMART.ERP.API.Controllers.v1
{
    [ApiVersion("1.0")]
    public class PurchaseBillController : BaseApiController
    {
        //[Authorize(Roles = "SuperAdmin, Admin, Manager, SalesAdvisor")]
        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] CreatePurchaseBillFromPurchaseOrderDetailPageCommand command)
        {
            return Ok(await Mediator.Send(command));
        }
    }
}
