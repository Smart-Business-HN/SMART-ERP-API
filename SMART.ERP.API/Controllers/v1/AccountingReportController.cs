using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using SMART.ERP.Application.Features.AccountingReportFeature.Queries;
using SMART.ERP.Application.Features.JournalEntryFeature.Queries;
using SMART.ERP.Domain.Enums;

namespace SMART.ERP.API.Controllers.v1
{
    [ApiVersion("1.0")]
    public class AccountingReportController : BaseApiController
    {
        [HttpGet("LibroMayor")]
        [Authorize(Roles = "SuperAdmin, Manager, Admin, Accountant")]
        [OutputCache(PolicyName = "cache_accounting_reports")]
        public async Task<IActionResult> LibroMayor([FromQuery] int ledgerAccountId, [FromQuery] DateTime fromDate, [FromQuery] DateTime toDate)
        {
            return Ok(await Mediator.Send(new LibroMayorQuery { LedgerAccountId = ledgerAccountId, FromDate = fromDate, ToDate = toDate }));
        }

        [HttpGet("BalanceComprobacion")]
        [Authorize(Roles = "SuperAdmin, Manager, Admin, Accountant")]
        [OutputCache(PolicyName = "cache_accounting_reports")]
        public async Task<IActionResult> BalanceComprobacion([FromQuery] DateTime fromDate, [FromQuery] DateTime toDate)
        {
            return Ok(await Mediator.Send(new BalanceComprobacionQuery { FromDate = fromDate, ToDate = toDate }));
        }

        [HttpGet("EstadoSituacionFinanciera")]
        [Authorize(Roles = "SuperAdmin, Manager, Admin, Accountant")]
        [OutputCache(PolicyName = "cache_accounting_reports")]
        public async Task<IActionResult> EstadoSituacionFinanciera([FromQuery] DateTime cutoffDate)
        {
            return Ok(await Mediator.Send(new EstadoSituacionFinancieraQuery { CutoffDate = cutoffDate }));
        }

        [HttpGet("EstadoResultados")]
        [Authorize(Roles = "SuperAdmin, Manager, Admin, Accountant")]
        [OutputCache(PolicyName = "cache_accounting_reports")]
        public async Task<IActionResult> EstadoResultados([FromQuery] DateTime fromDate, [FromQuery] DateTime toDate)
        {
            return Ok(await Mediator.Send(new EstadoResultadosQuery { FromDate = fromDate, ToDate = toDate }));
        }

        [HttpGet("LibroDiario")]
        [Authorize(Roles = "SuperAdmin, Manager, Admin, Accountant")]
        public async Task<IActionResult> LibroDiario([FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate,
            [FromQuery] int pageNumber = 0, [FromQuery] int pageSize = 25, [FromQuery] bool all = false)
        {
            return Ok(await Mediator.Send(new GetAllJournalEntriesQuery
            {
                FromDate = fromDate,
                ToDate = toDate,
                PageNumber = pageNumber,
                PageSize = pageSize == 0 ? 25 : pageSize,
                Status = JournalEntryStatus.Posted,
                All = all
            }));
        }
    }
}
