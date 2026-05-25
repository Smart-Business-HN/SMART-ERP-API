using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMART.ERP.Application.Features.JournalEntryFeature.Commands.CreateJournalEntryCommand;
using SMART.ERP.Application.Features.JournalEntryFeature.Commands.DeleteJournalEntryCommand;
using SMART.ERP.Application.Features.JournalEntryFeature.Commands.PostJournalEntryCommand;
using SMART.ERP.Application.Features.JournalEntryFeature.Commands.ReverseJournalEntryCommand;
using SMART.ERP.Application.Features.JournalEntryFeature.Commands.UpdateJournalEntryCommand;
using SMART.ERP.Application.Features.JournalEntryFeature.Queries;
using SMART.ERP.Domain.Enums;

namespace SMART.ERP.API.Controllers.v1
{
    [ApiVersion("1.0")]
    public class JournalEntryController : BaseApiController
    {
        [HttpPost("Create")]
        [Authorize(Roles = "SuperAdmin, Manager, Admin, Accountant")]
        public async Task<IActionResult> Create([FromBody] CreateJournalEntryCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        [HttpPost("Post/{id}")]
        [Authorize(Roles = "SuperAdmin, Manager, Admin, Accountant")]
        public async Task<IActionResult> Post(int id)
        {
            return Ok(await Mediator.Send(new PostJournalEntryCommand { Id = id }));
        }

        [HttpPost("Reverse/{id}")]
        [Authorize(Roles = "SuperAdmin, Manager, Admin, Accountant")]
        public async Task<IActionResult> Reverse(int id, [FromBody] ReverseJournalEntryCommand command)
        {
            command.Id = id;
            return Ok(await Mediator.Send(command));
        }

        [HttpPut("Update/{id}")]
        [Authorize(Roles = "SuperAdmin, Manager, Admin, Accountant")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateJournalEntryCommand command)
        {
            if (id != command.Id)
                return BadRequest(new { message = "Ocurrió un problema con el id de este registro" });
            return Ok(await Mediator.Send(command));
        }

        [HttpDelete("Delete/{id}")]
        [Authorize(Roles = "SuperAdmin, Manager, Admin, Accountant")]
        public async Task<IActionResult> Delete(int id)
        {
            return Ok(await Mediator.Send(new DeleteJournalEntryCommand { Id = id }));
        }

        [HttpGet("GetById/{id}")]
        [Authorize(Roles = "SuperAdmin, Manager, Admin, Accountant")]
        public async Task<IActionResult> GetById(int id)
        {
            return Ok(await Mediator.Send(new GetJournalEntryByIdQuery { Id = id }));
        }

        [HttpGet("GetAll")]
        [Authorize(Roles = "SuperAdmin, Manager, Admin, Accountant")]
        public async Task<IActionResult> GetAll([FromQuery] int pageNumber, [FromQuery] int pageSize,
            [FromQuery] string? parameter, [FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate,
            [FromQuery] JournalEntryStatus? status, [FromQuery] bool all = false)
        {
            return Ok(await Mediator.Send(new GetAllJournalEntriesQuery
            {
                PageNumber = pageNumber,
                PageSize = pageSize == 0 ? 10 : pageSize,
                Parameter = parameter,
                FromDate = fromDate,
                ToDate = toDate,
                Status = status,
                All = all
            }));
        }
    }
}
