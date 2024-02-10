using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMART.ERP.Application.Features.PurchaseBillFeature.Commands.CreatePurchaseBillCommand;
using SMART.ERP.Application.Features.PurchaseBillFeature.Commands.CreatePurchaseBillFromPurchaseOrderDetailPageCommand;

namespace SMART.ERP.API.Controllers.v1
{
    [ApiVersion("1.0")]
    public class PurchaseBillController : BaseApiController
    {
        [HttpPost("CreatePurchaseBillFromPurchaseOrderDetailPage")]
        [Authorize(Roles = "SuperAdmin, Admin, Manager, SalesAdvisor")]
        public async Task<IActionResult> CreatePurchaseBillFromPurchaseOrderDetailPage([FromBody] CreatePurchaseBillFromPurchaseOrderDetailPageCommand command)
        {
            return Ok(await Mediator.Send(command));
        }
        [HttpPost("Create")]
        [Authorize(Roles = "SuperAdmin, Admin, Manager, SalesAdvisor")]
        public async Task<IActionResult> Create([FromBody] CreatePurchaseBillCommand command)
        {
            return Ok(await Mediator.Send(command));
        }
    }
}
