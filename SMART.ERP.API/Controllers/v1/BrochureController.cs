using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMART.ERP.Application.Features.BrochureFeature.Queries;

namespace SMART.ERP.API.Controllers.v1
{
    [ApiVersion("1.0")]
    [Authorize]
    public class BrochureController : BaseApiController
    {
        /// <summary>
        /// Cuántos productos y páginas produciría el brochure, y cuántos quedarían
        /// fuera por no tener precio en la lista elegida.
        /// </summary>
        [HttpPost("Preview")]
        public async Task<IActionResult> Preview([FromBody] GetBrochurePreviewQuery query)
        {
            return Ok(await Mediator.Send(query));
        }

        /// <summary>
        /// Genera y descarga el brochure en PDF.
        ///
        /// Es POST y no GET (a diferencia de los otros endpoints de archivo, que son
        /// GET) porque el filtro es multi-selección: tres arreglos de ids más opciones
        /// no caben cómodamente en una query string.
        /// </summary>
        [HttpPost("Generate")]
        public async Task<IActionResult> Generate([FromBody] GenerateBrochureQuery query)
        {
            var pdf = await Mediator.Send(query);
            var fileName = $"brochure-smart-business-{DateTime.Now:yyyyMMdd-HHmm}.pdf";
            return File(pdf, "application/pdf", fileName);
        }
    }
}
