using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMART.ERP.Application.Features.BaseProductFeature.Queries;
using SMART.ERP.Application.Services.HeaderService;

namespace SMART.ERP.API.Controllers.v2
{
    [ApiVersion("2.0")]
    public class ProductController : BaseApiController
    {

        [HttpGet("GetBySlug/{slug}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(string slug)
        {
            return Ok(await Mediator.Send(new GetBaseProductBySlugQuery { Slug = slug }));
        }
    }
}
