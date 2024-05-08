using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using SMART.ERP.Application.Features.DailyClosinFeature.Commands;
using SMART.ERP.Application.Features.DailyClosinFeature.Queries;
using SMART.ERP.Application.Parameters;

namespace SMART.ERP.API.Controllers.v1
{
    [ApiVersion("1.0")]
    public class DailyClosingController : BaseApiController
    {
        [HttpPost("Create")]
        [Authorize(Roles = "SuperAdmin, Manager, SalesAdvisor")]
        public async Task<IActionResult> Create([FromBody] CreateDailyCloseCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        [HttpGet("GetAll")]
        [AllowAnonymous]
        [OutputCache(PolicyName = "cache_dailyClosing")]
        public async Task<IActionResult> GetAll([FromQuery] RequestParameter filter)
        {
            return Ok(await Mediator.Send(new GetAllDailyClosingQuery
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
