using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMART.ERP.Application.Features.NonBillableExpensePaymentFeature.Commands.CreateNonBillableExpensePaymentCommand;
using SMART.ERP.Application.Features.NonBillableExpensePaymentFeature.Commands.DeleteNonBillableExpensePaymentCommand;
using SMART.ERP.Application.Features.NonBillableExpensePaymentFeature.Commands.UpdateNonBillableExpensePaymentCommand;

namespace SMART.ERP.API.Controllers.v1
{
    [ApiVersion("1.0")]
    public class NonBillableExpensePaymentController : BaseApiController
    {
        //[HttpGet("GetByNonBillableExpenseId/{nonBillableExpenseId}")]
        //[Authorize(Roles = "SuperAdmin, Manager, Admin, SalesAdvisor")]
        //public async Task<IActionResult> GetByInvoiceId(int nonBillableExpenseId)
        //{
        //    return Ok(await Mediator.Send(new GetNonBillableExpensePaymentsByNonBillableExpenseIdQuery { NonBillableExpenseId = nonBillableExpenseId }));
        //}

        [HttpPost("Create")]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateNonBillableExpensePaymentCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        [HttpDelete("Delete/{id}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Delete(int id)
        {
            return Ok(await Mediator.Send(new DeleteNonBillableExpensePaymentCommand { Id = id }));
        }

        [HttpPut("Update/{id}")]
        [Authorize]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateNonBillableExpensePaymentCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest("Ocurrio un error con el id de este registro");
            }
            return Ok(await Mediator.Send(command));
        }
    }
}
