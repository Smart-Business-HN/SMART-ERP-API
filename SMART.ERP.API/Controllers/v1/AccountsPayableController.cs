using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMART.ERP.Application.Features.AccountsPayableFeature.Queries;
using SMART.ERP.Application.Parameters;

namespace SMART.ERP.API.Controllers.v1;

public class AccountsPayableController : BaseApiController
{
    [HttpGet("GetAll")]
    [Authorize]
    public async Task<IActionResult> GetAll([FromQuery] RequestParameter filter)
    {
        return Ok(await Mediator.Send(new GetAllAccountsPayableQuery
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