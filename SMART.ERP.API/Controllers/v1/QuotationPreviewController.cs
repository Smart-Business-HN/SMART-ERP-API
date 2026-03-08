using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMART.ERP.Application.Features.QuotationPreviewFeature.Commands.CreateQuotationClientCommentCommand;
using SMART.ERP.Application.Features.QuotationPreviewFeature.Commands.CreateQuotationItemObservationCommand;
using SMART.ERP.Application.Features.QuotationPreviewFeature.Queries;
using SMART.ERP.Application.Services.QuotationPdfService;

namespace SMART.ERP.API.Controllers.v1
{
    [ApiVersion("1.0")]
    [AllowAnonymous]
    public class QuotationPreviewController : BaseApiController
    {
        private readonly IQuotationPdfService _pdfService;

        public QuotationPreviewController(IQuotationPdfService pdfService)
        {
            _pdfService = pdfService;
        }

        [HttpGet("GetByToken/{token}")]
        public async Task<IActionResult> GetByToken(Guid token)
        {
            return Ok(await Mediator.Send(new GetQuotationByTokenQuery { AccessToken = token }));
        }

        [HttpGet("GetComments/{token}")]
        public async Task<IActionResult> GetComments(Guid token)
        {
            return Ok(await Mediator.Send(new GetQuotationCommentsByTokenQuery { AccessToken = token }));
        }

        [HttpGet("GetVersions/{token}")]
        public async Task<IActionResult> GetVersions(Guid token, [FromQuery] int pageNumber = 0, [FromQuery] int pageSize = 10)
        {
            return Ok(await Mediator.Send(new GetQuotationVersionsByTokenQuery
            {
                AccessToken = token,
                PageNumber = pageNumber,
                PageSize = pageSize
            }));
        }

        [HttpPost("AddComment")]
        public async Task<IActionResult> AddComment([FromBody] CreateQuotationClientCommentCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        [HttpPost("AddItemObservation")]
        public async Task<IActionResult> AddItemObservation([FromBody] CreateQuotationItemObservationCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        [HttpGet("DownloadPdf/{token}")]
        public async Task<IActionResult> DownloadPdf(Guid token)
        {
            var quotationResponse = await Mediator.Send(new GetQuotationByTokenQuery { AccessToken = token });
            if (quotationResponse.Data == null)
            {
                return NotFound();
            }

            var pdfBytes = await _pdfService.GeneratePdfAsync(quotationResponse.Data);
            return File(pdfBytes, "application/pdf", $"{quotationResponse.Data.QuotationCode}.pdf");
        }
    }
}
