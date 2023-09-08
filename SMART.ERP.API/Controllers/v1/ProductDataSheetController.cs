using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMART.ERP.Application.Features.ProductDataSheetFeature.Commands.DeleteProductDataSheetCommand;
using SMART.ERP.Application.Features.ProductDataSheetFeature.Queries;
using SMART.ERP.Application.Features.ProductDataSheetFeature.Commands.CreateProductDataSheetCommand;
using SMART.ERP.Application.Features.ProductDataSheetFeature.Commands.UpdateProductDataSheetCommand;

namespace SMART.ERP.API.Controllers.v1
{
    [ApiVersion("1.0")]
    public class ProductDataSheetController : BaseApiController
    {
        [HttpGet("GetById/{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(int id)
        {
            return Ok(await Mediator.Send(new GetProductDataSheetByIdQuery { Id = id }));
        }

        [HttpGet("GetAll")]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await Mediator.Send(new GetAllProductDataSheetsQuery()));
        }

        [HttpPost("Create")]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateProductDataSheetCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        [HttpPut("Update/{id}")]
        [Authorize(Roles = "SuperAdmin, Manager, CommunityManager, Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateProductDataSheetCommand command)
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
            return Ok(await Mediator.Send(new DeleteProductDataSheetCommand { Id = id }));
        }
    }
}
