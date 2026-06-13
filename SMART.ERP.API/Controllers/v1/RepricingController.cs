using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMART.ERP.Application.Features.RepricingFeature.Commands.ApplyProposedPriceCommand;
using SMART.ERP.Application.Features.RepricingFeature.Commands.DeleteCompetitorSourceCommand;
using SMART.ERP.Application.Features.RepricingFeature.Commands.RecheckProductNowCommand;
using SMART.ERP.Application.Features.RepricingFeature.Commands.UpdateRepricingSettingsCommand;
using SMART.ERP.Application.Features.RepricingFeature.Commands.UpsertCompetitorSourceCommand;
using SMART.ERP.Application.Features.RepricingFeature.Commands.UpsertRepricingRuleCommand;
using SMART.ERP.Application.Features.RepricingFeature.Queries;

namespace SMART.ERP.API.Controllers.v1
{
    [ApiVersion("1.0")]
    public class RepricingController : BaseApiController
    {
        // ---------- Fuentes de competencia ----------

        [HttpGet("GetSourcesByProduct/{productId}")]
        [Authorize(Roles = "SuperAdmin, Manager, Admin, CommunityManager")]
        public async Task<IActionResult> GetSourcesByProduct(int productId)
        {
            return Ok(await Mediator.Send(new GetCompetitorSourcesByProductQuery { ProductId = productId }));
        }

        [HttpPost("UpsertSource")]
        [Authorize(Roles = "SuperAdmin, Manager, Admin")]
        public async Task<IActionResult> UpsertSource([FromBody] UpsertCompetitorSourceCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        [HttpDelete("DeleteSource/{id}")]
        [Authorize(Roles = "SuperAdmin, Manager, Admin")]
        public async Task<IActionResult> DeleteSource(int id)
        {
            return Ok(await Mediator.Send(new DeleteCompetitorSourceCommand { Id = id }));
        }

        // ---------- Reglas por producto ----------

        [HttpGet("GetRule/{productId}")]
        [Authorize(Roles = "SuperAdmin, Manager, Admin, CommunityManager")]
        public async Task<IActionResult> GetRule(int productId)
        {
            return Ok(await Mediator.Send(new GetRepricingRuleQuery { ProductId = productId }));
        }

        [HttpPost("UpsertRule")]
        [Authorize(Roles = "SuperAdmin, Manager, Admin")]
        public async Task<IActionResult> UpsertRule([FromBody] UpsertRepricingRuleCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        // ---------- Configuración global ----------

        [HttpGet("GetSettings")]
        [Authorize(Roles = "SuperAdmin, Manager, Admin")]
        public async Task<IActionResult> GetSettings()
        {
            return Ok(await Mediator.Send(new GetRepricingSettingsQuery()));
        }

        [HttpPut("UpdateSettings")]
        [Authorize(Roles = "SuperAdmin, Manager, Admin")]
        public async Task<IActionResult> UpdateSettings([FromBody] UpdateRepricingSettingsCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        // ---------- Dashboard / bitácora ----------

        [HttpGet("GetDashboard")]
        [Authorize(Roles = "SuperAdmin, Manager, Admin, CommunityManager")]
        public async Task<IActionResult> GetDashboard()
        {
            return Ok(await Mediator.Send(new GetRepricingDashboardQuery()));
        }

        [HttpGet("GetPriceChangeLog")]
        [Authorize(Roles = "SuperAdmin, Manager, Admin, CommunityManager")]
        public async Task<IActionResult> GetPriceChangeLog([FromQuery] int? productId, [FromQuery] int pageNumber = 0, [FromQuery] int pageSize = 20)
        {
            return Ok(await Mediator.Send(new GetPriceChangeLogQuery
            {
                ProductId = productId,
                PageNumber = pageNumber,
                PageSize = pageSize
            }));
        }

        // ---------- Acciones ----------

        [HttpPost("RecheckNow/{productId}")]
        [Authorize(Roles = "SuperAdmin, Manager, Admin")]
        public async Task<IActionResult> RecheckNow(int productId)
        {
            return Ok(await Mediator.Send(new RecheckProductNowCommand { ProductId = productId }));
        }

        [HttpPost("ApplyProposed/{logId}")]
        [Authorize(Roles = "SuperAdmin, Manager, Admin")]
        public async Task<IActionResult> ApplyProposed(int logId)
        {
            return Ok(await Mediator.Send(new ApplyProposedPriceCommand { PriceChangeLogId = logId }));
        }
    }
}
