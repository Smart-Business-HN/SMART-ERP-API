using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMART.ERP.Application.DTOs.VirtualStock;
using SMART.ERP.Application.Services.VirtualStock;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.API.Controllers.v1
{
    public class ImportCsvRequest
    {
        public int ProviderId { get; set; }
        public int WarehouseId { get; set; }
        public IFormFile File { get; set; } = null!;
    }

    [ApiVersion("1.0")]
    [Authorize]
    public class VirtualStockController : BaseApiController
    {
        private readonly IVirtualStockService _virtualStockService;

        public VirtualStockController(IVirtualStockService virtualStockService)
        {
            _virtualStockService = virtualStockService;
        }

        [HttpPost("import/csv")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(Response<object>), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> ImportCsv([FromForm] ImportCsvRequest request)
        {
            if (request.File == null || request.File.Length == 0)
            {
                return BadRequest(new Response<object>("Archivo no proporcionado o vacío"));
            }

            if (!request.File.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest(new Response<object>("Solo se permiten archivos CSV"));
            }

            var userName = User.Identity?.Name ?? "Unknown";

            using var stream = request.File.OpenReadStream();
            var result = await _virtualStockService.ImportStockFromCsvAsync(
                request.ProviderId,
                request.WarehouseId,
                stream,
                request.File.FileName,
                userName);

            return Ok(BuildImportResponse(result));
        }

        [HttpPost("import/excel")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(Response<object>), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> ImportExcel([FromForm] ImportCsvRequest request)
        {
            if (request.File == null || request.File.Length == 0)
            {
                return BadRequest(new Response<object>("Archivo no proporcionado o vacío"));
            }

            var fileName = request.File.FileName;
            if (!fileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase) &&
                !fileName.EndsWith(".xls", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest(new Response<object>("Solo se permiten archivos Excel (.xlsx/.xls)"));
            }

            var userName = User.Identity?.Name ?? "Unknown";

            using var stream = request.File.OpenReadStream();
            var result = await _virtualStockService.ImportStockFromExcelAsync(
                request.ProviderId,
                request.WarehouseId,
                stream,
                fileName,
                userName);

            return Ok(BuildImportResponse(result));
        }

        // Construye la respuesta con la forma que espera el frontend (ImportResult): succeeded, importedCount,
        // errorCount, errors[], message. Compartida por la importación CSV y Excel.
        private static Response<object> BuildImportResponse(VirtualStockImport result)
        {
            var message = $"Importación completada. {result.SuccessfulImports} productos importados correctamente, {result.FailedImports} errores.";
            var errors = (result.ImportDetails ?? new List<VirtualStockImportDetail>())
                .Where(d => !d.WasSuccessful)
                .Select(d => new { row = 0, productCode = d.ProductCode, errorMessage = d.ErrorMessage })
                .ToList();

            var data = new
            {
                succeeded = result.SuccessfulImports > 0,
                importedCount = result.SuccessfulImports,
                errorCount = result.FailedImports,
                errors,
                message,
                id = result.Id,
                fileName = result.FileName,
                totalProducts = result.TotalProducts,
                importDate = result.ImportDate
            };

            return new Response<object>(data, message);
        }

        // providerId <= 0 devuelve el historial de todos los proveedores (usado por la pestaña de historial
        // cuando aún no se ha seleccionado un proveedor).
        [HttpGet("import-history/{providerId}")]
        [ProducesResponseType(typeof(PagedResponse<List<VirtualStockImportHistoryItemDto>>), 200)]
        public async Task<IActionResult> GetImportHistory(
            int providerId,
            [FromQuery] int pageNumber = 0,
            [FromQuery] int pageSize = 10)
        {
            var history = await _virtualStockService.GetImportHistoryAsync(providerId, pageNumber, pageSize);
            return Ok(history);
        }

        [HttpGet("availability/{productId}")]
        [ProducesResponseType(typeof(Response<object>), 200)]
        public async Task<IActionResult> GetProductAvailability(int productId)
        {
            var availability = await _virtualStockService.GetProductAvailabilityAsync(productId);
            return Ok(new Response<object>(availability, "Disponibilidad obtenida exitosamente"));
        }
    }
}
