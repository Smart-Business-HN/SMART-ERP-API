using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMART.ERP.Application.Features.QuoteProductFeature.Commands.DeleteQuoteProductCommand;
using SMART.ERP.Application.Features.QuoteProductFeature.Queries;
using SMART.ERP.Application.Features.QuoteProductFeature.Commands.CreateQuoteProductCommand;
using SMART.ERP.Application.Features.QuoteProductFeature.Commands.UpdateQuoteProductCommand;
using SMART.ERP.Application.Features.QuoteProductFeature.Commands.UpdateStatusQuoteProductCommand;
using Asp.Versioning;

namespace SMART.ERP.API.Controllers.v1
{
    [ApiVersion("1.0")]
    public class QuoteProductController : BaseApiController
    {
        [HttpGet("GetById/{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(int id)
        {
            return Ok(await Mediator.Send(new GetQuoteProductByIdQuery { Id = id }));
        }

        [HttpGet("GetAll")]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await Mediator.Send(new GetAllQuoteProductsQuery()));
        }

        [HttpPost("Create")]
        [Authorize(Roles = "SuperAdmin, Manager, SalesAdvisor, Admin")]
        public async Task<IActionResult> Create([FromBody] CreateQuoteProductCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        [HttpPut("Update/{id}")]
        [Authorize(Roles = "SuperAdmin, Manager, SalesAdvisor, Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateQuoteProductCommand command)
        {
            if (id != command.Id)
                return BadRequest(new { message = "Ocurrio un problema con el id de este registro" });
            return Ok(await Mediator.Send(command));
        }

        [HttpDelete("Delete/{id}")]
        [Authorize(Roles = "SuperAdmin, Manager, Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            return Ok(await Mediator.Send(new DeleteQuoteProductCommand { Id = id }));
        }

        [HttpPut("UpdateStatus/{id}")]
        [Authorize(Roles = "SuperAdmin, Manager, SalesAdvisor, Admin")]
        public async Task<IActionResult> Deactivate(int id, [FromBody] UpdateStatusQuoteProductCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest(new { message = "Ocurrio un problema con el id de este registro" });
            }
            return Ok(await Mediator.Send(command));
        }
    }
}
