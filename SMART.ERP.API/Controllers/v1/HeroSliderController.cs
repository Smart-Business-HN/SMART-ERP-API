using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMART.ERP.Application.Features.BaseProductFeature.Queries;
using SMART.ERP.Application.Features.HeroSliderFeature.Commands.CreateHeroSliderCommand;
using SMART.ERP.Application.Features.HeroSliderFeature.Commands.DeleteHeroSliderCommand;
using SMART.ERP.Application.Features.HeroSliderFeature.Commands.DeleteProductSliderCommand;
using SMART.ERP.Application.Features.HeroSliderFeature.Commands.UpdateCategoryPositionCommand;
using SMART.ERP.Application.Features.HeroSliderFeature.Commands.UpdateHeroSliderCommand;
using SMART.ERP.Application.Features.HeroSliderFeature.Queries;

namespace SMART.ERP.API.Controllers.v1
{
    [ApiVersion("1.0")]
    public class HeroSliderController : BaseApiController
    {
        [HttpGet("GetAllWithCategory")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await Mediator.Send(new GetHeroSliderByCategoryQuery()));
        }

        [HttpGet("GetAllFromCategory/{id}")]
        public async Task<IActionResult> GetAllFromCategory(int id)
        {
            return Ok(await Mediator.Send(new GetAllProductByCategoryQuery { CategoryId = id }));
        }

        [HttpPost("Create")]
        [Authorize(Roles = "SuperAdmin, Manager, CommunityManager, Admin")]
        public async Task<IActionResult> Create([FromBody] CreateHeroSliderCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        [HttpPut("Update/{id}")]
        [Authorize(Roles = "SuperAdmin, Manager, CommunityManager, Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateHeroSliderCommand command)
        {
            if (id != command.Id)
                return BadRequest(new { message = "Ocurrio un problema con el id de este regitro" });
            return Ok(await Mediator.Send(command));
        }

        [HttpPut("UpdateCategoryPos/{id}")]
        [Authorize(Roles = "SuperAdmin, Manager, CommunityManager, Admin")]
        public async Task<IActionResult> UpdateCategoryPos(int id, [FromBody] UpdateCategoryPositionCommand command)
        {
            if (id != command.Id)
                return BadRequest(new { message = "Ocurrio un problema con el id de este regitro" });
            return Ok(await Mediator.Send(command));
        }

        [HttpDelete("Delete/{categoryId}")]
        [Authorize(Roles = "SuperAdmin, Manager, CommunityManager, Admin")]
        public async Task<IActionResult> Delete(int categoryId)
        {
            return Ok(await Mediator.Send(new DeleteHeroSliderCommand { Id = categoryId }));
        }

        [HttpDelete("DeleteProduct/{id}")]
        [Authorize(Roles = "SuperAdmin, Manager, CommunityManager, Admin")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            return Ok(await Mediator.Send(new DeleteProductSliderCommand { Id = id }));
        }
    }
}
