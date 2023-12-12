using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMART.ERP.Application.Features.BillPaymentFeature.Commands.CreateBillPaymentCommand;
using SMART.ERP.Application.Features.BillPaymentFeature.Commands.DeleteBillPaymentCommand;
using SMART.ERP.Application.Features.BillPaymentFeature.Commands.UpdateBillPaymentCommand;
using SMART.ERP.Application.Features.BillPaymentFeature.Queries;
using SMART.ERP.Application.Parameters;

namespace SMART.ERP.API.Controllers.v1
{
    [ApiVersion("1.0")]
    public class BillPaymentController : BaseApiController
    {
        [HttpGet("GetByInvoiceId/{invoiceId}")]
        [Authorize(Roles = "SuperAdmin, Manager, Admin, SalesAdvisor")]
        public async Task<IActionResult> GetByInvoiceId(int invoiceId)
        {
            return Ok(await Mediator.Send(new GetBillPaymentsByInvoiceIdQuery { InvoiceId = invoiceId }));
        }

        [HttpPost("Create")]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateBillPaymentCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        [HttpDelete("Delete/{id}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Delete(int id)
        {
            return Ok(await Mediator.Send(new DeleteBillPaymentCommand { Id = id }));
        }

        [HttpPut("Update/{id}")]
        [Authorize]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateBillPaymentCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest("Ocurrio un error con el id de este registro");
            }
            return Ok(await Mediator.Send(command));
        }
    }
}
