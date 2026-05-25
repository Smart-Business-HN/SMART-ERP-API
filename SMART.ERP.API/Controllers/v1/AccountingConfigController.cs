using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMART.ERP.Application.Features.AccountingConfigFeature.Commands;
using SMART.ERP.Application.Features.AccountingConfigFeature.Queries;

namespace SMART.ERP.API.Controllers.v1
{
    [ApiVersion("1.0")]
    public class AccountingConfigController : BaseApiController
    {
        [HttpGet("GetSettings")]
        [Authorize(Roles = "SuperAdmin, Manager, Admin, Accountant")]
        public async Task<IActionResult> GetSettings() => Ok(await Mediator.Send(new GetAccountingSettingsQuery()));

        [HttpPost("ToggleAutoPosting")]
        [Authorize(Roles = "SuperAdmin, Manager, Admin, Accountant")]
        public async Task<IActionResult> ToggleAutoPosting([FromBody] ToggleAutoPostingCommand command) => Ok(await Mediator.Send(command));

        [HttpGet("GetMappings")]
        [Authorize(Roles = "SuperAdmin, Manager, Admin, Accountant")]
        public async Task<IActionResult> GetMappings() => Ok(await Mediator.Send(new GetAccountingMappingsQuery()));

        [HttpPut("UpdateMapping")]
        [Authorize(Roles = "SuperAdmin, Manager, Admin, Accountant")]
        public async Task<IActionResult> UpdateMapping([FromBody] UpdateAccountingMappingCommand command) => Ok(await Mediator.Send(command));

        [HttpGet("GetBankAccountMappings")]
        [Authorize(Roles = "SuperAdmin, Manager, Admin, Accountant")]
        public async Task<IActionResult> GetBankAccountMappings() => Ok(await Mediator.Send(new GetBankAccountMappingsQuery()));

        [HttpPut("SetBankAccountMapping")]
        [Authorize(Roles = "SuperAdmin, Manager, Admin, Accountant")]
        public async Task<IActionResult> SetBankAccountMapping([FromBody] SetBankAccountMappingCommand command) => Ok(await Mediator.Send(command));

        [HttpGet("GetExpenseAccountMappings")]
        [Authorize(Roles = "SuperAdmin, Manager, Admin, Accountant")]
        public async Task<IActionResult> GetExpenseAccountMappings() => Ok(await Mediator.Send(new GetExpenseAccountMappingsQuery()));

        [HttpPut("SetExpenseAccountMapping")]
        [Authorize(Roles = "SuperAdmin, Manager, Admin, Accountant")]
        public async Task<IActionResult> SetExpenseAccountMapping([FromBody] SetExpenseAccountMappingCommand command) => Ok(await Mediator.Send(command));
    }
}
