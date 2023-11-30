using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using SMART.ERP.Application.Features.GenderFeature.Queries;

namespace SMART.ERP.API.Controllers.v1
{
    [ApiVersion("1.0")]
    public class GenderController : BaseApiController
    {
        [HttpGet("GetById/{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(int id)
        {
            return Ok(await Mediator.Send(new GetGenderByIdQuery { Id = id }));
        }

        [HttpGet("GetAll")]
        [AllowAnonymous]
        [OutputCache(PolicyName = "cache_genders")]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await Mediator.Send(new GetAllGendersQuery()));
        }

        [HttpGet("ClientGetAll")]
        [Authorize]
        public async Task<IActionResult> ClientGetAll()
        {
            return Ok(await Mediator.Send(new GetAllClientGenderQuery()));
        }
    }
}
