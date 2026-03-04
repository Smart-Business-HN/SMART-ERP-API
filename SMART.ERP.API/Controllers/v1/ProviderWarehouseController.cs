using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMART.ERP.Application.Features.ProviderWarehouseFeature.Commands.CreateProviderWarehouseCommand;
using SMART.ERP.Application.Features.ProviderWarehouseFeature.Commands.DeleteProviderWarehouseCommand;
using SMART.ERP.Application.Features.ProviderWarehouseFeature.Queries;

namespace SMART.ERP.API.Controllers.v1
{
    [ApiVersion("1.0")]
    public class ProviderWarehouseController : BaseApiController
    {
        [HttpGet("GetAll")]
        [Authorize(Roles = "SuperAdmin, Manager, Admin")]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await Mediator.Send(new GetAllProviderWarehousesQuery()));
        }

        [HttpGet("GetByProvider/{providerId}")]
        [Authorize(Roles = "SuperAdmin, Manager, Admin")]
        public async Task<IActionResult> GetByProvider(int providerId)
        {
            return Ok(await Mediator.Send(new GetProviderWarehousesByProviderQuery { ProviderId = providerId }));
        }

        [HttpPost("Create")]
        [Authorize(Roles = "SuperAdmin, Manager, Admin")]
        public async Task<IActionResult> Create([FromBody] CreateProviderWarehouseCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        [HttpDelete("Delete/{id}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Delete(int id)
        {
            return Ok(await Mediator.Send(new DeleteProviderWarehouseCommand { Id = id }));
        }
    }
}
