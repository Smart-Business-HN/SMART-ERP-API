using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMART.ERP.Application.Features.DocumentTypeFeature.Commands.DeleteDocumentTypeCommand;
using SMART.ERP.Application.Features.DocumentTypeFeature.Queries;
using SMART.ERP.Application.Parameters;
using SMART.ERP.API.Controllers;
using SMART.ERP.Application.Features.DocumentTypeFeature.Commands.CreateDocumentTypeCommand;
using SMART.ERP.Application.Features.DocumentTypeFeature.Commands.UpdateDocumentTypeCommand;

namespace SMART.ERP.API.Controllers.v1
{
    [ApiVersion("1.0")]
    public class DocumentTypeController : BaseApiController
    {
        [HttpPost("Create")]
        [Authorize(Roles = "SuperAdmin, Manager, Admin")]
        public async Task<IActionResult> Create([FromBody] CreateDocumentTypeCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        [HttpGet("GetById/{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(int id)
        {
            return Ok(await Mediator.Send(new GetDocumentTypeByIdQuery { Id = id }));
        }

        [HttpGet("GetAll")]
        [Authorize]
        public async Task<IActionResult> GetAll([FromQuery] RequestParameter filter)
        {
            return Ok(await Mediator.Send(new GetAllDocumentTypesQuery
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
        [Authorize(Roles = "SuperAdmin, Manager, Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateDocumentTypeCommand command)
        {
            if (id != command.Id)
                return BadRequest(new { message = "Ocurrio un problema con el id de este regitro" });
            return Ok(await Mediator.Send(command));
        }

        [HttpDelete("Delete/{id}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Delete(int id)
        {
            return Ok(await Mediator.Send(new DeleteDocumentTypeCommand { Id = id }));
        }
    }
}
