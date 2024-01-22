using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using SMART.ERP.Application.Features.ClientSocialReasonFeature.Queries;

namespace SMART.ERP.API.Controllers.v1
{
    [ApiVersion("1.0")]
    [Authorize]
    public class CustomerSocialReasonController : BaseApiController
    {
        [HttpGet("GetAll")]
        [OutputCache (PolicyName = "cache_socialReasons")]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await Mediator.Send(new GetAllSocialReasonQuery()));
        }
    }
}
