using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMART.ERP.Application.Features.PaymentMethodFeature.Commands.CreatePaymentMethodCommand;
using SMART.ERP.Application.Features.PaymentMethodFeature.Commands.DeletePaymentMethodCommand;
using SMART.ERP.Application.Features.PaymentMethodFeature.Commands.UpdatePaymentMethodCommand;
using SMART.ERP.Application.Features.PaymentMethodFeature.Queries;

namespace SMART.ERP.API.Controllers.v2
{
    [ApiVersion("2.0")]
    [Authorize]
    public class PaymentMethodController : BaseApiController
    {
        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] CreatePaymentMethodCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        [HttpGet("GetAll/{ecommerceUserId}")]
        public async Task<IActionResult> GetAll(Guid ecommerceUserId, [FromQuery] string? parameter, [FromQuery] int pageNumber = 0, [FromQuery] int pageSize = 10, [FromQuery] string? order = "asc", [FromQuery] string? column = "Alias", [FromQuery] bool all = false)
        {
            return Ok(await Mediator.Send(new GetAllPaymentMethodQuery
            {
                EcommerceUserId = ecommerceUserId,
                Parameter = parameter,
                PageNumber = pageNumber,
                PageSize = pageSize,
                Order = order,
                Column = column,
                All = all
            }));
        }

        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            return Ok(await Mediator.Send(new GetPaymentMethodByIdQuery { Id = id }));
        }

        [HttpPut("Update/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdatePaymentMethodCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest("El ID no coincide con el ID de la ruta.");
            }
            return Ok(await Mediator.Send(command));
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            return Ok(await Mediator.Send(new DeletePaymentMethodCommand { Id = id }));
        }
    }
}
