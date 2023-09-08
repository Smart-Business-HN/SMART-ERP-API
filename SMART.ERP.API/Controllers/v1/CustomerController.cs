using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMART.ERP.Application.Features.CustomerFeature.Queries;
using SMART.ERP.Application.Parameters;
using SMART.ERP.Application.Features.CustomerFeature.Commands.CreateCustomerCommand;
using SMART.ERP.Application.Features.CustomerFeature.Commands.ImportCustomerCommand;
using SMART.ERP.Application.Features.CustomerFeature.Commands.LoginCustomerCommand;
using SMART.ERP.Application.Features.CustomerFeature.Commands.UpdateCustomerCommand;

namespace SMART.ERP.API.Controllers.v1
{
    [ApiVersion("1.0")]
    public class CustomerController : BaseApiController
    {
        [HttpGet("GetById/{id}")]
        [Authorize(Roles = "SuperAdmin, Manager, Admin, CommunityManager, SalesAdvisor")]
        public async Task<IActionResult> GetById(Guid id)
        {
            return Ok(await Mediator.Send(new GetCustomerByIdQuery { Id = id }));
        }

        [HttpGet("GetAll")]
        [Authorize(Roles = "SuperAdmin, Manager, Admin, CommunityManager, SalesAdvisor")]
        public async Task<IActionResult> GetAll([FromQuery] RequestParameter filter)
        {
            return Ok(await Mediator.Send(new GetAllCustomersQuery()
            {
                Parameter = filter.Parameter,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                Order = filter.Order,
                Column = filter.Column,
                All = filter.All
            }));
        }

        [HttpGet("GetAllBasic")]
        [Authorize(Roles = "SuperAdmin, Admin, Manager, CommunityManager, SalesAdvisor")]
        public async Task<IActionResult> GetAllBasic()
        {
            return Ok(await Mediator.Send(new GetAllBasicInfoCustomerQuery()));
        }

        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginCustomerCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        [HttpPost("Create")]
        [Authorize(Roles = "SuperAdmin, Admin, Manager, CommunityManager, SalesAdvisor")]
        public async Task<IActionResult> Create([FromBody] CreateCustomerCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        [HttpPut("Update/{id}")]
        [Authorize(Roles = "SuperAdmin, Admin, Manager, CommunityManager, SalesAdvisor")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCustomerCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest("Ocurrio un error con el id de este registro");
            }
            return Ok(await Mediator.Send(command));
        }

        [HttpGet("GetAllHN")]
        [Authorize(Roles = "SuperAdmin, Admin, Manager, CommunityManager, SalesAdvisor")]
        public async Task<IActionResult> GetAll([FromQuery] string? Parameter, [FromQuery] int PageNumber)
        {
            return Ok(await Mediator.Send(new GetAllHNClientQuery()
            {
                Parameter = Parameter,
                PageNumber = PageNumber
            }));
        }

        [HttpPost("Import")]
        [Authorize(Roles = "SuperAdmin, Admin, Manager, CommunityManager, SalesAdvisor")]
        public async Task<IActionResult> Import([FromBody] ImportCustomerCommand command)
        {
            return Ok(await Mediator.Send(command));
        }
    }
}
