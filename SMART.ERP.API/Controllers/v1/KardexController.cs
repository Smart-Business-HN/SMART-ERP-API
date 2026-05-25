using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMART.ERP.Application.Features.KardexFeature.Queries;

namespace SMART.ERP.API.Controllers.v1
{
    [ApiVersion("1.0")]
    public class KardexController : BaseApiController
    {
        [HttpGet("GetProductKardex")]
        [Authorize]
        public async Task<IActionResult> GetProductKardex([FromQuery] int productId, [FromQuery] int? warehouseId,
            [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate, [FromQuery] bool includeCancellations = false)
        {
            return Ok(await Mediator.Send(new GetProductKardexQuery
            {
                ProductId = productId,
                WarehouseId = warehouseId,
                StartDate = startDate,
                EndDate = endDate,
                IncludeCancellations = includeCancellations
            }));
        }

        [HttpGet("ExportPdf")]
        [Authorize]
        public async Task<IActionResult> ExportPdf([FromQuery] int productId, [FromQuery] int? warehouseId,
            [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate, [FromQuery] bool includeCancellations = false)
        {
            var pdf = await Mediator.Send(new GenerateProductKardexPdfQuery
            {
                ProductId = productId,
                WarehouseId = warehouseId,
                StartDate = startDate,
                EndDate = endDate,
                IncludeCancellations = includeCancellations
            });
            return File(pdf, "application/pdf", $"kardex_{productId}.pdf");
        }

        [HttpGet("ExportExcel")]
        [Authorize]
        public async Task<IActionResult> ExportExcel([FromQuery] int productId, [FromQuery] int? warehouseId,
            [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate, [FromQuery] bool includeCancellations = false)
        {
            var excel = await Mediator.Send(new GenerateProductKardexExcelQuery
            {
                ProductId = productId,
                WarehouseId = warehouseId,
                StartDate = startDate,
                EndDate = endDate,
                IncludeCancellations = includeCancellations
            });
            return File(excel, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"kardex_{productId}.xlsx");
        }
    }
}
