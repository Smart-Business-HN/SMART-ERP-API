using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using SMART.ERP.Application.Features.MonthlyPurchaseDeclarationFeature.Commands.CreateMonthlyPurchaseDeclarationCommand;

namespace SMART.ERP.API.Controllers.v1
{
    [ApiVersion("1.0")]
    public class MonthlyPurchaseDeclarationController : BaseApiController
    {
        [HttpPost("Create")]
        public async Task<IActionResult> Send([FromBody] CreateMonthlyPurchaseDeclarationCommand command)
        {
            return Ok(await Mediator.Send(command));
        }
    }
}
