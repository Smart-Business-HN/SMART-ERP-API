using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMART.ERP.Application.Features.MonthlyPurchaseDeclarationFeature.Commands.CreateMonthlyPurchaseDeclarationCommand;
using SMART.ERP.Application.Features.MonthlyPurchaseDeclarationFeature.Queries;
using SMART.ERP.Application.Parameters;

namespace SMART.ERP.API.Controllers.v1
{
    [ApiVersion("1.0")]
    public class MonthlyPurchaseDeclarationController : BaseApiController
    {
        [HttpPost("Create")]
        [Authorize]
        public async Task<IActionResult> Send([FromBody] CreateMonthlyPurchaseDeclarationCommand command)
        {
            return Ok(await Mediator.Send(command));
        }
        [HttpGet("GetAll")]
        [Authorize]
        public async Task<IActionResult> GetAll([FromQuery] RequestParameter filter)
        {
            return Ok(await Mediator.Send(new GetAllMonthlyPurchaseDeclarationQuery
            {
                Parameter = filter.Parameter,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                Order = filter.Order,
                Column = filter.Column,
                All = filter.All
            }));
        }
    }
}
