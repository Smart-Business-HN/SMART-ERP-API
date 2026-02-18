using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMART.ERP.Application.Features.ShippingCostConfigurationFeature.Commands.CreateShippingCostConfigurationCommand;
using SMART.ERP.Application.Features.ShippingCostConfigurationFeature.Commands.DeleteShippingCostConfigurationCommand;
using SMART.ERP.Application.Features.ShippingCostConfigurationFeature.Commands.UpdateShippingCostConfigurationCommand;
using SMART.ERP.Application.Features.ShippingCostConfigurationFeature.Queries;
using SMART.ERP.Application.Parameters;

namespace SMART.ERP.API.Controllers.v1
{
    [ApiVersion("1.0")]
    public class ShippingCostConfigurationController : BaseApiController
    {
        [HttpGet("GetById/{id}")]
        [Authorize(Roles = "SuperAdmin, Manager, Admin")]
        public async Task<IActionResult> GetById(int id)
        {
            return Ok(await Mediator.Send(new GetShippingCostConfigurationByIdQuery { Id = id }));
        }

        [HttpGet("GetAll")]
        [Authorize(Roles = "SuperAdmin, Manager, Admin")]
        public async Task<IActionResult> GetAll([FromQuery] RequestParameter filter)
        {
            return Ok(await Mediator.Send(new GetAllShippingCostConfigurationsQuery
            {
                Parameter = filter.Parameter,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                Order = filter.Order,
                Column = filter.Column,
                All = filter.All
            }));
        }

        [HttpGet("GetByWarehouse/{warehouseId}")]
        [Authorize(Roles = "SuperAdmin, Manager, Admin")]
        public async Task<IActionResult> GetByWarehouse(int warehouseId)
        {
            return Ok(await Mediator.Send(new GetShippingCostConfigurationsByWarehouseQuery { WarehouseId = warehouseId }));
        }

        [HttpGet("GetByProvider/{providerId}")]
        [Authorize(Roles = "SuperAdmin, Manager, Admin")]
        public async Task<IActionResult> GetByProvider(int providerId)
        {
            return Ok(await Mediator.Send(new GetShippingCostConfigurationsByProviderQuery { ProviderId = providerId }));
        }

        [HttpPost("Create")]
        [Authorize(Roles = "SuperAdmin, Manager, Admin")]
        public async Task<IActionResult> Create([FromBody] CreateShippingCostConfigurationCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        [HttpDelete("Delete/{id}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Delete(int id)
        {
            return Ok(await Mediator.Send(new DeleteShippingCostConfigurationCommand { Id = id }));
        }

        [HttpPut("Update/{id}")]
        [Authorize(Roles = "SuperAdmin, Manager, Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateShippingCostConfigurationCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest("Ocurrió un error con el id de este registro");
            }
            return Ok(await Mediator.Send(command));
        }
    }
}
