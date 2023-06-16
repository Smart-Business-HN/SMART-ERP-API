using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMART.ERP.Application.Features.OpinionFeature.Commands.DeleteOpinionCommand;
using SMART.ERP.Application.Features.OpinionFeature.Queries;
using SMART.ERP.Application.Parameters;
using SMART.ERP.Application.Services.HeaderService;
using SMART.ERP.API.Controllers;
using SMART.ERP.Application.Features.OpinionFeature.Commands.CreateOpinionCommand;
using SMART.ERP.Application.Features.OpinionFeature.Commands.UpdateOpinionCommand;

namespace SMART.ERP.API.Controllers.v1
{
    [ApiVersion("1.0")]
    public class OpinionController : BaseApiController
    {
        private readonly IHeaderService _headerService;

        public OpinionController(IHeaderService headerService)
        {
            _headerService = headerService;
        }

        [HttpGet("GetById/{id}")]
        [Authorize(Roles = "SuperAdmin, Manager, CommunityManager, Admin")]
        public async Task<IActionResult> GetById(int id)
        {
            return Ok(await Mediator.Send(new GetOpinionByIdQuery { Id = id }));
        }

        [HttpGet("GetAll")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll([FromQuery] RequestParameter filter)
        {
            if (!_headerService.VerificatedSecretKey())
                return Unauthorized();

            return Ok(await Mediator.Send(new GetAllOpinionsQuery()
            {
                Parameter = filter.Parameter,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                Order = filter.Order,
                Column = filter.Column,
                All = filter.All
            }));
        }

        [HttpPost("Create")]
        [Authorize(Roles = "SuperAdmin, Manager, CommunityManager, Admin")]
        public async Task<IActionResult> Create([FromBody] CreateOpinionCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        [HttpPut("Update/{id}")]
        [Authorize(Roles = "SuperAdmin, Manager, CommunityManager, Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateOpinionCommand command)
        {
            if (id != command.Id)
                return BadRequest(new
                {
                    message = "Ocurrio un problema con el id de este regitro"
                });
            return Ok(await Mediator.Send(command));
        }

        [HttpDelete("Delete/{id}")]
        [Authorize(Roles = "SuperAdmin, Manager, CommunityManager, Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            return Ok(await Mediator.Send(new DeleteOpinionCommand { Id = id }));
        }
    }
}
