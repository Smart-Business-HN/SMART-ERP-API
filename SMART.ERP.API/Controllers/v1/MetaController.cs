using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMART.ERP.Application.Features.MetaFeature.Commands.ConversationCommand;

namespace SMART.ERP.API.Controllers.v1
{
    [ApiVersion("1.0")]
    public class MetaController : BaseApiController
    {
        [AllowAnonymous]
        [HttpPost("webhook")]
        public async Task<IActionResult> webhook([FromBody] ConversationCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        [AllowAnonymous]
        [HttpGet("webhook")]
        public IActionResult getWebhook()
        {
            var req = Request.Query.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            Microsoft.Extensions.Primitives.StringValues challenge;
            Microsoft.Extensions.Primitives.StringValues verify_token;
            req.TryGetValue("hub.challenge", out challenge);
            req.TryGetValue("hub.verify_token", out verify_token);
            if (verify_token.ToString() == "#Pl4t1n0M0t0r52023*")
            {
                return Ok(challenge.ToString());
            }
            return BadRequest();
        }
    }
}
