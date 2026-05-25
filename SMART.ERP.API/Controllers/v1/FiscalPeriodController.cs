using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using SMART.ERP.Application.Features.FiscalPeriodFeature.Commands.CloseFiscalYearCommand;
using SMART.ERP.Application.Features.FiscalPeriodFeature.Commands.CreateFiscalYearCommand;
using SMART.ERP.Application.Features.FiscalPeriodFeature.Commands.SetFiscalPeriodStatusCommand;
using SMART.ERP.Application.Features.FiscalPeriodFeature.Queries;

namespace SMART.ERP.API.Controllers.v1
{
    [ApiVersion("1.0")]
    public class FiscalPeriodController : BaseApiController
    {
        [HttpPost("CreateFiscalYear")]
        [Authorize(Roles = "SuperAdmin, Manager, Admin, Accountant")]
        public async Task<IActionResult> CreateFiscalYear([FromBody] CreateFiscalYearCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        [HttpPost("SetPeriodStatus")]
        [Authorize(Roles = "SuperAdmin, Manager, Admin, Accountant")]
        public async Task<IActionResult> SetPeriodStatus([FromBody] SetFiscalPeriodStatusCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        [HttpPost("CloseFiscalYear/{fiscalYearId}")]
        [Authorize(Roles = "SuperAdmin, Manager, Admin, Accountant")]
        public async Task<IActionResult> CloseFiscalYear(int fiscalYearId)
        {
            return Ok(await Mediator.Send(new CloseFiscalYearCommand { FiscalYearId = fiscalYearId }));
        }

        [HttpGet("GetAllFiscalYears")]
        [Authorize(Roles = "SuperAdmin, Manager, Admin, Accountant")]
        [OutputCache(PolicyName = "cache_fiscal_periods")]
        public async Task<IActionResult> GetAllFiscalYears()
        {
            return Ok(await Mediator.Send(new GetAllFiscalYearsQuery()));
        }

        [HttpGet("GetCurrentPeriod")]
        [Authorize(Roles = "SuperAdmin, Manager, Admin, Accountant")]
        public async Task<IActionResult> GetCurrentPeriod([FromQuery] DateTime? date)
        {
            return Ok(await Mediator.Send(new GetCurrentFiscalPeriodQuery { Date = date }));
        }
    }
}
