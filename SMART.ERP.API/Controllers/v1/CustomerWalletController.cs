using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMART.ERP.Application.Features.UserFeature.Queries;

namespace SMART.ERP.API.Controllers.v1
{
    [ApiVersion("1.0")]
    public class CustomerWalletController : BaseApiController
    {
        [HttpGet("GetByUserId/{id}")]
        [Authorize]
        public async Task<IActionResult> GetByUserId(Guid id)
        {
            return Ok(await Mediator.Send(new GetUserWalletByIdQuery { Id = id }));
        }
    }
}
