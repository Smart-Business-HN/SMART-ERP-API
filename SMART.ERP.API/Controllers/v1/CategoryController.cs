using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using SMART.ERP.Application.Features.CategoryFeature.Commands.CreateCategoryCommand;
using SMART.ERP.Application.Features.CategoryFeature.Commands.DeleteCategoryCommand;
using SMART.ERP.Application.Features.CategoryFeature.Commands.UpdateCategoryCommand;
using SMART.ERP.Application.Features.CategoryFeature.Queries;
using SMART.ERP.Application.Parameters;
using SMART.ERP.Application.Services.HeaderService;

namespace SMART.ERP.API.Controllers.v1
{
    [ApiVersion("1.0")]
    public class CategoryController : BaseApiController
    {
        private readonly IHeaderService _headerService;

        public CategoryController(IHeaderService headerService)
        {
            _headerService = headerService;
        }

        [HttpGet("GetById/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            return Ok(await Mediator.Send(new GetCategoryByIdQuery { Id = id }));
        }

        [HttpGet("GetAll")]
        [AllowAnonymous]
        [OutputCache (PolicyName ="cache_categories")]
        public async Task<IActionResult> GetAll([FromQuery] RequestParameter filter)
        {
            if (!_headerService.VerificatedSecretKey())
                return Unauthorized();

            return Ok(await Mediator.Send(new GetAllCategoriesQuery
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
        public async Task<IActionResult> Create([FromBody] CreateCategoryCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        [HttpPut("Update/{id}")]
        [Authorize(Roles = "SuperAdmin, Manager, CommunityManager, Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCategoryCommand command)
        {
            if (id != command.Id)
                return BadRequest(new { message = "Ocurrio un problema con el id de este regitro" });
            return Ok(await Mediator.Send(command));
        }

        [HttpDelete("Delete/{id}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Delete(int id)
        {
            return Ok(await Mediator.Send(new DeleteCategoryCommand { Id = id }));
        }
    }
}
