using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMART.ERP.Application.Features.ProspectFeature.Queries;
using SMART.ERP.Application.Parameters;
using SMART.ERP.API.Controllers;
using SMART.ERP.Application.Features.ProspectFeature.Commands.ConvertProspectCommand;
using SMART.ERP.Application.Features.ProspectFeature.Commands.CreateProspectCommand;
using SMART.ERP.Application.Features.ProspectFeature.Commands.CreateProspectXpressCommand;
using SMART.ERP.Application.Features.ProspectFeature.Commands.UpdateProspectCommand;
using SMART.ERP.Application.Features.ProspectFeature.Commands.UpdateProspectStepCommand;

namespace SMART.ERP.API.Controllers.v1
{

    public class ProspectController : BaseApiController
    {
        [HttpPost("Create")]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateProspectCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        [HttpPut("UpdateStep")]
        [Authorize]
        public async Task<IActionResult> UpdateStep([FromBody] UpdateProspectStepCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        [HttpPut("Update/{id}")]
        [Authorize]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProspectCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest("Ocurrio un error con el id de este registro");
            }
            return Ok(await Mediator.Send(command));
        }

        [HttpPut("ConvertProspect")]
        [Authorize]
        public async Task<IActionResult> ConvertProspect([FromBody] ConvertProspectCommand command)
        {
            return Ok(await Mediator.Send(command));
        }


        [HttpPost("CreateProspectXpress")]
        [AllowAnonymous]
        public async Task<IActionResult> CreateProspectXpress([FromBody] CreateProspectXpressCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        [HttpGet("GetAll")]
        [Authorize]
        public async Task<IActionResult> GetAll([FromQuery] RequestParameter filter)
        {
            return Ok(await Mediator.Send(new GetAllProspectQuery
            {
                Parameter = filter.Parameter,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                Order = filter.Order,
                Column = filter.Column,
                All = filter.All
            }));
        }

        [HttpGet("GetById/{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(Guid id)
        {
            return Ok(await Mediator.Send(new GetProspectByIdQuery { Id = id }));
        }
    }
}
