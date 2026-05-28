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
        [HttpGet("GeneralLedger")]
        [Authorize(Roles = "SuperAdmin, Manager, Admin, Accountant")]
        [OutputCache(PolicyName = "cache_accounting_reports")]
        public async Task<IActionResult> GeneralLedger([FromQuery] int ledgerAccountId, [FromQuery] DateTime fromDate, [FromQuery] DateTime toDate)
        {
            return Ok(await Mediator.Send(new GeneralLedgerQuery { LedgerAccountId = ledgerAccountId, FromDate = fromDate, ToDate = toDate }));
        }

        [HttpGet("TrialBalance")]
        [Authorize(Roles = "SuperAdmin, Manager, Admin, Accountant")]
        [OutputCache(PolicyName = "cache_accounting_reports")]
        public async Task<IActionResult> TrialBalance([FromQuery] DateTime fromDate, [FromQuery] DateTime toDate)
        {
            return Ok(await Mediator.Send(new TrialBalanceQuery { FromDate = fromDate, ToDate = toDate }));
        }

        [HttpGet("BalanceSheet")]
        [Authorize(Roles = "SuperAdmin, Manager, Admin, Accountant")]
        [OutputCache(PolicyName = "cache_accounting_reports")]
        public async Task<IActionResult> BalanceSheet([FromQuery] DateTime cutoffDate)
        {
            return Ok(await Mediator.Send(new BalanceSheetQuery { CutoffDate = cutoffDate }));
        }

        [HttpGet("IncomeStatement")]
        [Authorize(Roles = "SuperAdmin, Manager, Admin, Accountant")]
        [OutputCache(PolicyName = "cache_accounting_reports")]
        public async Task<IActionResult> IncomeStatement([FromQuery] DateTime fromDate, [FromQuery] DateTime toDate)
        {
            return Ok(await Mediator.Send(new IncomeStatementQuery { FromDate = fromDate, ToDate = toDate }));
        }

        [HttpGet("Journal")]
        [Authorize(Roles = "SuperAdmin, Manager, Admin, Accountant")]
        public async Task<IActionResult> Journal([FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate,
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
