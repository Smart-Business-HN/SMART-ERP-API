using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMART.ERP.Application.Features.BaseProductFeature.Queries;
using SMART.ERP.Application.Services.HeaderService;

namespace SMART.ERP.API.Controllers.v2
{
    [ApiVersion("2.0")]
    public class ProductController : BaseApiController
    {
        private readonly IHeaderService _headerService;

        public ProductController(IHeaderService headerService)
        {
            _headerService = headerService;
        }
        [HttpGet("GetById/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(string slug)
        {
            if (!_headerService.VerificatedSecretKey())
                return Unauthorized();

            return Ok(await Mediator.Send(new GetBaseProductBySlugQuery { Slug = slug }));
        }
    }
}
