using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMART.ERP.Application.Features.InvoiceFeature.Commands.CreateInvoiceByQuotationIdCommand;
using SMART.ERP.Application.Features.InvoiceFeature.Commands.CreateInvoiceCommand;
using SMART.ERP.Application.Features.InvoiceFeature.Commands.UpdateInvoiceCommand;
using SMART.ERP.Application.Features.InvoiceFeature.Queries;
using SMART.ERP.Application.Parameters;

namespace SMART.ERP.API.Controllers.v1
{
    [ApiVersion("1.0")]
    public class InvoiceController : BaseApiController
    {
        [HttpPost("Create")]
        [Authorize(Roles = "SuperAdmin, Admin, Manager, SalesAdvisor")]
        public async Task<IActionResult> Create([FromBody] CreateInvoiceCommand command)
        {
            return Ok(await Mediator.Send(command));
        }
        [HttpPost("CreateByQuotationId/{quotationId}")]
        [Authorize(Roles = "SuperAdmin, Admin, Manager, SalesAdvisor")]
        public async Task<IActionResult> CreateByQuotationId(int quotationId,[FromBody] CreateInvoiceByQuotationIdCommand command)
        {
            if (quotationId != command.QuotationId)
            {
                return BadRequest("Ocurrio un error con el id de este registro");
            }
            return Ok(await Mediator.Send(command));
        }
        [HttpGet("GetById/{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(int id)
        {
            return Ok(await Mediator.Send(new GetInvoiceByIdQuery { Id = id }));
        }
        [HttpGet("GetAll")]
        [Authorize]
        public async Task<IActionResult> GetAll([FromQuery] RequestParameter filter)
        {
            return Ok(await Mediator.Send(new GetAllInvoiceQuery
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
        [Authorize(Roles = "SuperAdmin, Admin, Manager, CommunityManager, SalesAdvisor")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateInvoiceCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest("Ocurrio un error con el id de este registro");
            }
            return Ok(await Mediator.Send(command));
        }
    }
}
