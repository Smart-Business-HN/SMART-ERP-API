using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMART.ERP.Application.Features.OpportunityFeature.Queries;
using SMART.ERP.Application.Features.QuotationFeature.Commands.CreateQuotationCommand;
using SMART.ERP.Application.Features.QuotationFeature.Queries;
using SMART.ERP.Application.Parameters;

namespace SMART.ERP.API.Controllers.v1
{
    [ApiVersion("1.0")]
    public class QuotationController : BaseApiController
    {
        [HttpGet("GetById/{id}")]
        //[Authorize]
        public async Task<IActionResult> GetById(int id)
        {
            return Ok(await Mediator.Send(new GetQuotationByIdQuery { Id = id }));
        }
        [HttpPost("Create")]
        //[Authorize(Roles = "SuperAdmin, Admin, Manager, CommunityManager, SalesAdvisor")]
        public async Task<IActionResult> Create([FromBody] CreateQuotationCommand command)
        {
            return Ok(await Mediator.Send(command));
        }
        [HttpGet("GetAll")]
        //[Authorize]
        public async Task<IActionResult> GetAll([FromQuery] RequestParameter filter)
        {
            return Ok(await Mediator.Send(new GetAllQuotationQuery()
            {
                Parameter = filter.Parameter,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                Order = filter.Order,
                Column = filter.Column,
                All = filter.All
            }));
        }
        //[HttpPut("Update/{id}")]
        //[Authorize(Roles = "SuperAdmin, Admin, Manager, CommunityManager, SalesAdvisor")]
        //public async Task<IActionResult> Update(Guid id, [FromBody] UpdateQuotationCommand command)
        //{
        //    if (id != command.Id)
        //    {
        //        return BadRequest("Ocurrio un error con el id de este registro");
        //    }
        //    return Ok(await Mediator.Send(command));
        //}

    }
}
