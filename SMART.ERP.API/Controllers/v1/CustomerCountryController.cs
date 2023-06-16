using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMART.ERP.Application.Features.ClientCountryFeature.Queries;
using SMART.ERP.API.Controllers;

namespace SMART.ERP.API.Controllers.v1
{
    [ApiVersion("1.0")]
    [Authorize]
    public class CustomerCountryController : BaseApiController
    {
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await Mediator.Send(new GetAllClientCountryQuery()));
        }
    }
}
