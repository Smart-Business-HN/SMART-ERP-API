using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMART.ERP.Application.Features.ReportFeature.Queries;
using SMART.ERP.Application.Parameters;

namespace SMART.ERP.API.Controllers.v1
{
    [ApiVersion("1.0")]
    [Authorize]
    public class PurchaseSuggestionController : BaseApiController
    {
        /// <summary>
        /// Sugerencias de compra para abastecimiento de inventario segun las unidades vendidas.
        /// Solo considera productos que tocan inventario (tangibles).
        /// </summary>
        [HttpGet("GetSuggestions")]
        public async Task<IActionResult> GetSuggestions([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate,
            [FromQuery] int coverageDays, [FromQuery] RequestParameter param)
        {
            return Ok(await Mediator.Send(new PurchaseSuggestionReportQuery
            {
                StartDate = startDate,
                EndDate = endDate,
                CoverageDays = coverageDays <= 0 ? 30 : coverageDays,
                PageNumber = param.PageNumber,
                PageSize = param.PageSize,
                All = param.All
            }));
        }
    }
}
