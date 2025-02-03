using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using SMART.ERP.Application.Features.MonthlyPurchaseDeclarationFeature.Commands.CreateMonthlyPurchaseDeclarationCommand;
using SMART.ERP.Application.Features.MonthlyPurchaseDeclarationFeature.Commands.DeleteMonthlyPurchaseDeclarationCommand;
using SMART.ERP.Application.Features.MonthlyPurchaseDeclarationFeature.Queries;
using SMART.ERP.Application.Parameters;

namespace SMART.ERP.API.Controllers.v1
{
    [ApiVersion("1.0")]
    public class MonthlyPurchaseDeclarationController : BaseApiController
    {
        [HttpPost("Create")]
        [Authorize(Roles = "SuperAdmin, Manager, Admin")]
        public async Task<IActionResult> Send([FromBody] CreateMonthlyPurchaseDeclarationCommand command)
        {
            return Ok(await Mediator.Send(command));
        }
        [HttpGet("GetAll")]
        [AllowAnonymous]
        [OutputCache(PolicyName = "cache_monthlyPurchaseDeclaration")]
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
        [HttpGet("GetById/{id}")]
        [Authorize(Roles = "SuperAdmin, Manager, Admin")]
        public async Task<IActionResult> GetById(int id)
        {
            return Ok(await Mediator.Send(new GetMonthlyPurchaseDeclarationByIdQuery { Id = id }));
        }
        [HttpDelete("Delete/{id}")]
        [Authorize(Roles = "SuperAdmin, Manager, Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            return Ok(await Mediator.Send(new DeleteMonthlyPurchaseDeclarationCommand { Id = id }));
        }
    }
}
