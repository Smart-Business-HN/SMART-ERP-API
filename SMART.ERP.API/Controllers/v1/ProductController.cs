using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMART.ERP.Application.Features.BaseProductFeature.Commands.DeleteBaseProductCommand;
using SMART.ERP.Application.Features.BaseProductFeature.Queries;
using SMART.ERP.Application.Parameters;
using SMART.ERP.Application.Services.HeaderService;
using SMART.ERP.Application.Features.BaseProductFeature.Commands.CreateBaseProductCommand;
using SMART.ERP.Application.Features.BaseProductFeature.Commands.UpdateBaseProductCommand;

namespace SMART.ERP.API.Controllers.v1
{
    [ApiVersion("1.0")]
    public class ProductController : BaseApiController
    {
        private readonly IHeaderService _headerService;

        public ProductController(IHeaderService headerService)
        {
            _headerService = headerService;
        }

        [HttpGet("GetById/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            if (!_headerService.VerificatedSecretKey())
                return Unauthorized();

            return Ok(await Mediator.Send(new GetBaseProductByIdQuery { Id = id }));
        }

        [HttpGet("GetByCategoryId/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetByCategoryId(int id)
        {
            if (!_headerService.VerificatedSecretKey())
                return Unauthorized();

            return Ok(await Mediator.Send(new GetAllProductByCategoryQuery { CategoryId = id }));
        }

        [HttpGet("GetBySubCategoryId/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetBySubCategoryId(int id)
        {
            if (!_headerService.VerificatedSecretKey())
                return Unauthorized();

            return Ok(await Mediator.Send(new GetAllProductBySubcategoryQuery { SubCategoryId = id }));
        }

        [HttpGet("GetBasicDetailById/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetBasicDetailById(int id)
        {
            if (!_headerService.VerificatedSecretKey())
                return Unauthorized();

            return Ok(await Mediator.Send(new GetBasicDetailProductById { Id = id }));
        }

        [HttpGet("GetAll")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll([FromQuery] RequestParameter filter)
        {
            if (!_headerService.VerificatedSecretKey())
                return Unauthorized();

            return Ok(await Mediator.Send(new GetAllBaseProductsQuery()
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
        public async Task<IActionResult> Create([FromBody] CreateBaseProductCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        [HttpPut("Update/{id}")]
        [Authorize(Roles = "SuperAdmin, Manager, CommunityManager, Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateBaseProductCommand command)
        {
            if (id != command.Id)
                return BadRequest(new
                {
                    message = "Ocurrio un problema con el id de este regitro"
                });
            return Ok(await Mediator.Send(command));
        }

        [HttpDelete("Delete/{id}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Delete(int id)
        {
            return Ok(await Mediator.Send(new DeleteBaseProductCommand { Id = id }));
        }
    }
}
