using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using SMART.ERP.Application.Features.DepartmentFeature.Queries;
using SMART.ERP.Application.Parameters;

namespace SMART.ERP.API.Controllers.v2;
[ApiVersion("2.0")]
public class DepartmentController : BaseApiController
{
    [HttpGet("GetAll")]
    [AllowAnonymous]
    [OutputCache(PolicyName = "cache_departments")]
    public async Task<IActionResult> GetAll([FromQuery] RequestParameter filter)
    {
        return Ok(await Mediator.Send(new GetAllDepartmentsQuery
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