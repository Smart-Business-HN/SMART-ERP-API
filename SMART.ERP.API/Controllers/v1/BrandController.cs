using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMART.ERP.Application.Features.BrandFeature.Commands.DeleteBrandCommand;
using SMART.ERP.Application.Features.BrandFeature.Queries;
using SMART.ERP.Application.Parameters;
using SMART.ERP.Application.Services.HeaderService;
using SMART.ERP.Application.Features.BrandFeature.Commands.CreateBrandCommand;
using SMART.ERP.Application.Features.BrandFeature.Commands.UpdateBrandCommand;

namespace SMART.ERP.API.Controllers.v1
{
    [ApiVersion("1.0")]
    public class BrandController : BaseApiController
    {
        private readonly IHeaderService _headerService;

        public BrandController(IHeaderService headerService)
        {
            _headerService = headerService;
        }

        [HttpPost("Create")]
        [Authorize(Roles = "SuperAdmin, Manager, CommunityManager, Admin")]
        public async Task<IActionResult> Create([FromBody] CreateBrandCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        [HttpGet("GetById/{id}")]
        [Authorize(Roles = "SuperAdmin, Manager, Admin, CommunityManager")]
        public async Task<IActionResult> GetById(int id)
        {
            return Ok(await Mediator.Send(new GetBrandByIdQuery { Id = id }));
        }

        [HttpGet("GetAll")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll([FromQuery] RequestParameter filter)
        {
            if (!_headerService.VerificatedSecretKey())
                return Unauthorized();

            return Ok(await Mediator.Send(new GetAllBrandQuery
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
        public async Task<IActionResult> Update(int id, [FromBody] UpdateBrandCommand command)
        {
            if (id != command.Id)
                return BadRequest(new { message = "Ocurrio un problema con el id de este regitro" });
            return Ok(await Mediator.Send(command));
        }

        [HttpDelete("Delete/{id}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Delete(int id)
        {
            return Ok(await Mediator.Send(new DeleteBrandCommand { Id = id }));
        }
    }
}
