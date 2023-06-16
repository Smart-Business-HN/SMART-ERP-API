using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMART.ERP.Application.Features.ProspectStepFeature.Queries;
using SMART.ERP.Application.Parameters;
using SMART.ERP.API.Controllers;

namespace SMART.ERP.API.Controllers.v1
{
    [Authorize]
    public class ProspectStepController : BaseApiController
    {
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll([FromQuery] RequestParameter filter)
        {
            return Ok(await Mediator.Send(new GetAllProspectStepQuery
            {
                Parameter = filter.Parameter,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                Order = filter.Order,
                Column = filter.Column,
                All = filter.All
            }));
        }
    }
}
