using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMART.ERP.Application.Features.RecurringInvoiceTemplateFeature.Commands.CreateRecurringInvoiceTemplateCommand;
using SMART.ERP.Application.Features.RecurringInvoiceTemplateFeature.Commands.UpdateRecurringInvoiceTemplateCommand;
using SMART.ERP.Application.Features.RecurringInvoiceTemplateFeature.Commands.ToggleRecurringInvoiceTemplateCommand;
using SMART.ERP.Application.Features.RecurringInvoiceTemplateFeature.Queries;
using SMART.ERP.Application.Parameters;

namespace SMART.ERP.API.Controllers.v1
{
    [ApiVersion("1.0")]
    public class RecurringInvoiceTemplateController : BaseApiController
    {
        [HttpPost("Create")]
        [Authorize(Roles = "SuperAdmin, Admin, Manager")]
        public async Task<IActionResult> Create([FromBody] CreateRecurringInvoiceTemplateCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        [HttpPut("Update/{id}")]
        [Authorize(Roles = "SuperAdmin, Admin, Manager")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateRecurringInvoiceTemplateCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest("Ocurrio un error con el id de este registro");
            }
            return Ok(await Mediator.Send(command));
        }

        [HttpPut("Toggle/{id}")]
        [Authorize(Roles = "SuperAdmin, Admin, Manager")]
        public async Task<IActionResult> Toggle(int id, [FromBody] ToggleRecurringInvoiceTemplateCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest("Ocurrio un error con el id de este registro");
            }
            return Ok(await Mediator.Send(command));
        }

        [HttpGet("GetById/{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(int id)
        {
            return Ok(await Mediator.Send(new GetRecurringInvoiceTemplateByIdQuery { Id = id }));
        }

        [HttpGet("GetAll")]
        [Authorize]
        public async Task<IActionResult> GetAll([FromQuery] RequestParameter filter)
        {
            return Ok(await Mediator.Send(new GetAllRecurringInvoiceTemplateQuery
            {
                Parameter = filter.Parameter,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                All = filter.All
            }));
        }

        [HttpGet("GetLogs/{templateId}")]
        [Authorize(Roles = "SuperAdmin, Admin, Manager")]
        public async Task<IActionResult> GetLogs(int templateId, [FromQuery] RequestParameter filter)
        {
            return Ok(await Mediator.Send(new GetRecurringInvoiceLogsByTemplateIdQuery
            {
                TemplateId = templateId,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize
            }));
        }
    }
}
