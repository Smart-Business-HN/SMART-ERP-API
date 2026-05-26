using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using SMART.ERP.Application.Features.CreditCardPaymentFeature.Commands.CreateCreditCardPaymentCommand;
using SMART.ERP.Application.Features.CreditCardPaymentFeature.Commands.DeleteCreditCardPaymentCommand;
using SMART.ERP.Application.Features.CreditCardPaymentFeature.Queries;
using SMART.ERP.Application.Parameters;

namespace SMART.ERP.API.Controllers.v1
{
    [ApiVersion("1.0")]
    public class CreditCardPaymentController : BaseApiController
    {
        [HttpPost("Create")]
        [Authorize(Roles = "SuperAdmin, Manager, Admin")]
        public async Task<IActionResult> Create([FromBody] CreateCreditCardPaymentCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        [HttpGet("GetById/{id}")]
        [Authorize(Roles = "SuperAdmin, Manager, Admin, Accountan")]
        public async Task<IActionResult> GetById(int id)
        {
            return Ok(await Mediator.Send(new GetCreditCardPaymentByIdQuery { Id = id }));
        }

        [HttpGet("GetAll")]
        [Authorize(Roles = "SuperAdmin, Manager, Admin, Accountan")]
        [OutputCache(PolicyName = "cache_creditCardPayment")]
        public async Task<IActionResult> GetAll([FromQuery] RequestParameter filter)
        {
            return Ok(await Mediator.Send(new GetAllCreditCardPaymentsQuery
            {
                Parameter = filter.Parameter,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                Order = filter.Order,
                Column = filter.Column,
                All = filter.All
            }));
        }

        [HttpDelete("Delete/{id}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Delete(int id)
        {
            return Ok(await Mediator.Send(new DeleteCreditCardPaymentCommand { Id = id }));
        }
    }
}
