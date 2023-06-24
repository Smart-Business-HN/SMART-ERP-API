using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SMART.ERP.Application.Features.BrandFeature.Commands.CreateBrandCommand;
using SMART.ERP.Application.Features.BrandFeature.Commands.UpdateBrandCommand;
using SMART.ERP.Application.Features.BrandFeature.Queries;
using SMART.ERP.Application.Features.CaiFeature.Commands.CreateCaiCommand;
using SMART.ERP.Application.Features.CaiFeature.Commands.UpdateCaiCommand;
using SMART.ERP.Application.Features.CaiFeature.Queries;
using SMART.ERP.Application.Parameters;
using SMART.ERP.Application.Services.HeaderService;

namespace SMART.ERP.API.Controllers.v1
{
    [ApiVersion("1.0")]
    public class CaiController : BaseApiController
    {
        private readonly IHeaderService _headerService;
        public CaiController (IHeaderService headerService)
        {
            _headerService = headerService;
        }
        [HttpPost("Create")]
        // [Authorize(Roles = "SuperAdmin, Manager, CommunityManager, Admin")]
        [AllowAnonymous]
        public async Task<IActionResult> Create([FromBody] CreateCaiCommand command)
        {
            return Ok(await Mediator.Send(command));
        }
        [HttpGet("GetById/{id}")]
        [Authorize(Roles = "SuperAdmin, Manager, Admin, CommunityManager, SalesAdvisor")]
        public async Task<IActionResult> GetById(int id)
        {
            return Ok(await Mediator.Send(new GetCaiByIdQuery { Id = id }));
        }

        [HttpGet("GetAll")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll([FromQuery] RequestParameter filter)
        {
            return Ok(await Mediator.Send(new GetAllCaisQuery
            {
                Parameter = filter.Parameter,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                Order = filter.Order,
                Column = filter.Column,
                All = filter.All
            }));
        }
        [HttpPut("Update/{id}")]
        [Authorize(Roles = "SuperAdmin, Manager, CommunityManager, Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCaiCommand command)
        {
            if (id != command.Id)
                return BadRequest(new { message = "Ocurrio un problema con el id de este regitro" });
            return Ok(await Mediator.Send(command));
        }
    }
}
