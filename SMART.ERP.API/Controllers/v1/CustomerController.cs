using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using SMART.ERP.Application.Features.CustomerFeature.Commands.CreateCustomerCommand;
using SMART.ERP.Application.Features.CustomerFeature.Commands.ImportCustomerCommand;
using SMART.ERP.Application.Features.CustomerFeature.Commands.LoginCustomerCommand;
using SMART.ERP.Application.Features.CustomerFeature.Commands.UpdateCustomerCommand;
using SMART.ERP.Application.Features.CustomerFeature.Queries;
using SMART.ERP.Application.Parameters;

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

        [HttpGet("GetSummary/{id}")]
        [Authorize(Roles = "SuperAdmin, Manager, Admin, CommunityManager, SalesAdvisor")]
        public async Task<IActionResult> GetSummary(Guid id)
        {
            return Ok(await Mediator.Send(new GetCustomerSummaryQuery { Id = id }));
        }

        [HttpGet("GetInvoices/{id}")]
        [Authorize(Roles = "SuperAdmin, Manager, Admin, CommunityManager, SalesAdvisor")]
        public async Task<IActionResult> GetInvoices(Guid id, [FromQuery] RequestParameter filter)
        {
            return Ok(await Mediator.Send(new GetCustomerInvoicesQuery
            {
                CustomerId = id,
                Parameter = filter.Parameter,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize
            }));
        }

        [HttpGet("GetQuotations/{id}")]
        [Authorize(Roles = "SuperAdmin, Manager, Admin, CommunityManager, SalesAdvisor")]
        public async Task<IActionResult> GetQuotations(Guid id, [FromQuery] RequestParameter filter)
        {
            return Ok(await Mediator.Send(new GetCustomerQuotationsQuery
            {
                CustomerId = id,
                Parameter = filter.Parameter,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize
            }));
        }

        [HttpGet("GetAll")]
        [OutputCache(PolicyName = "cache_customers")]
        [AllowAnonymous]
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

        [HttpPost("Import")]
        [Authorize(Roles = "SuperAdmin, Admin, Manager, CommunityManager, SalesAdvisor")]
        public async Task<IActionResult> Import([FromBody] ImportCustomerCommand command)
        {
            return Ok(await Mediator.Send(command));
        }
    }
}
