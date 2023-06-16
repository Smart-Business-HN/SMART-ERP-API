using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMART.ERP.Application.Features.CustomerMachineryFeature.Commands.DeleteCustomerMachineryCommand;
using SMART.ERP.Application.Features.CustomerMachineryFeature.Queries;
using SMART.ERP.Application.Parameters;
using SMART.ERP.API.Controllers;
using SMART.ERP.Application.Features.CustomerMachineryFeature.Commands.CreateCustomerMachineryCommand;
using SMART.ERP.Application.Features.CustomerMachineryFeature.Commands.UpdateCustomerMachineryCommand;

namespace SMART.ERP.API.Controllers.v1
{
    [ApiVersion("1.0")]
    public class CustomerMachineryController : BaseApiController
    {
        [HttpPost("Create")]
        [Authorize(Roles = "SuperAdmin, Manager, SalesAdvisor")]
        public async Task<IActionResult> Create([FromBody] CreateCustomerMachineryCommand command)
        {

            return Ok(await Mediator.Send(command));
        }
        [HttpGet("GetAll")]
        [Authorize(Roles = "SuperAdmin, Manager, SalesAdvisor")]
        public async Task<IActionResult> GetAll([FromQuery] RequestParameter filter)
        {
            return Ok(await Mediator.Send(new GetAllCustomerMachineryQuery()
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
        [Authorize(Roles = "SuperAdmin, Manager, SalesAdvisor")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCustomerMachineryCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest("Ocurrio un error con el id de este registro");
            }
            return Ok(await Mediator.Send(command));
        }
        [HttpDelete("Delete/{id}")]
        [Authorize(Roles = "SuperAdmin, Manager, SalesAdvisor")]
        public async Task<IActionResult> Delete(int id)
        {
            return Ok(await Mediator.Send(new DeleteCustomerMachineryCommand { Id = id }));
        }
    }
}
