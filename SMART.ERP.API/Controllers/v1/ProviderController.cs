using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using SMART.ERP.Application.Features.ProviderFeature.Commands.CreateProviderCommand;
using SMART.ERP.Application.Features.ProviderFeature.Commands.DeleteProviderCommand;
using SMART.ERP.Application.Features.ProviderFeature.Commands.UpdateProviderCommand;
using SMART.ERP.Application.Features.ProviderFeature.Commands.UpdateProviderCreditCommand;
using SMART.ERP.Application.Features.ProviderFeature.Queries;
using SMART.ERP.Application.Parameters;

namespace SMART.ERP.API.Controllers.v1
{
    [ApiVersion("1.0")]
    public class ProviderController : BaseApiController
    {
        [HttpGet("GetById/{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(int id)
        {
            return Ok(await Mediator.Send(new GetProviderByIdQuery { Id = id }));
        }

        [HttpGet("GetSummary/{id}")]
        [Authorize]
        public async Task<IActionResult> GetSummary(int id)
        {
            return Ok(await Mediator.Send(new GetProviderSummaryQuery { Id = id }));
        }

        [HttpGet("GetPurchaseBills/{id}")]
        [Authorize]
        public async Task<IActionResult> GetPurchaseBills(int id, [FromQuery] RequestParameter filter)
        {
            return Ok(await Mediator.Send(new GetProviderPurchaseBillsQuery
            {
                ProviderId = id,
                Parameter = filter.Parameter,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize
            }));
        }

        [HttpGet("GetPurchaseOrders/{id}")]
        [Authorize]
        public async Task<IActionResult> GetPurchaseOrders(int id, [FromQuery] RequestParameter filter)
        {
            return Ok(await Mediator.Send(new GetProviderPurchaseOrdersQuery
            {
                ProviderId = id,
                Parameter = filter.Parameter,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize
            }));
        }

        [HttpPut("UpdateCredit/{id}")]
        [Authorize(Roles = "SuperAdmin, Manager")]
        public async Task<IActionResult> UpdateCredit(int id, [FromBody] UpdateProviderCreditCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest(new { message = "Ocurrio un problema con el id de este registro" });
            }
            return Ok(await Mediator.Send(command));
        }

        [HttpGet("GetAll")]
        [AllowAnonymous]
        [OutputCache(PolicyName = "cache_providers")]
        public async Task<IActionResult> GetAll([FromQuery] RequestParameter filter)
        {
            return Ok(await Mediator.Send(new GetAllProvidersQuery()
            {
                Parameter = filter.Parameter,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                Order = filter.Order,
                Column = filter.Column,
                All = filter.All
            }));
        }

        [HttpPost("Create")]
        [Authorize(Roles = "SuperAdmin, Manager")]
        public async Task<IActionResult> Create([FromBody] CreateProviderCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        [HttpPut("Update/{id}")]
        [Authorize(Roles = "SuperAdmin, Manager")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateProviderCommand command)
        {
            if (id != command.Id)
                return BadRequest(new
                {
                    message = "Ocurrio un problema con el id de este regitro"
                });
            return Ok(await Mediator.Send(command));
        }

        [HttpDelete("Delete/{id}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Delete(int id)
        {
            return Ok(await Mediator.Send(new DeleteProviderCommand { Id = id }));
        }
    }
}
