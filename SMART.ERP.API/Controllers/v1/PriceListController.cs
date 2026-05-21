using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using SMART.ERP.Application.Features.PriceListFeature.Commands.AssignPriceListToCustomerCommand;
using SMART.ERP.Application.Features.PriceListFeature.Commands.AssignPriceListToCustomerTypeCommand;
using SMART.ERP.Application.Features.PriceListFeature.Commands.BulkSetPriceListItemsCommand;
using SMART.ERP.Application.Features.PriceListFeature.Commands.ClonePriceListCommand;
using SMART.ERP.Application.Features.PriceListFeature.Commands.CreatePriceListCommand;
using SMART.ERP.Application.Features.PriceListFeature.Commands.DeletePriceListCommand;
using SMART.ERP.Application.Features.PriceListFeature.Commands.DeletePriceListItemCommand;
using SMART.ERP.Application.Features.PriceListFeature.Commands.RegeneratePriceListFromCostCommand;
using SMART.ERP.Application.Features.PriceListFeature.Commands.SetDefaultPriceListCommand;
using SMART.ERP.Application.Features.PriceListFeature.Commands.SetPriceListItemCommand;
using SMART.ERP.Application.Features.PriceListFeature.Commands.UpdatePriceListCommand;
using SMART.ERP.Application.Features.PriceListFeature.Queries;
using SMART.ERP.Application.Parameters;

namespace SMART.ERP.API.Controllers.v1
{
    [ApiVersion("1.0")]
    public class PriceListController : BaseApiController
    {
        [HttpPost("Create")]
        [Authorize(Roles = "SuperAdmin, Manager, Admin")]
        public async Task<IActionResult> Create([FromBody] CreatePriceListCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        [HttpGet("GetAll")]
        [Authorize(Roles = "SuperAdmin, Manager, Admin, CommunityManager")]
        [OutputCache(PolicyName = "cache_pricelists")]
        public async Task<IActionResult> GetAll([FromQuery] RequestParameter filter)
        {
            return Ok(await Mediator.Send(new GetAllPriceListsQuery
            {
                Parameter = filter.Parameter,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                Order = filter.Order,
                Column = filter.Column,
                All = filter.All
            }));
        }

        [HttpGet("GetById/{id}")]
        [Authorize(Roles = "SuperAdmin, Manager, Admin, CommunityManager")]
        public async Task<IActionResult> GetById(int id)
        {
            return Ok(await Mediator.Send(new GetPriceListByIdQuery { Id = id }));
        }

        [HttpPut("Update/{id}")]
        [Authorize(Roles = "SuperAdmin, Manager, Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdatePriceListCommand command)
        {
            if (id != command.Id)
                return BadRequest(new { message = "Ocurrió un problema con el id" });
            return Ok(await Mediator.Send(command));
        }

        [HttpDelete("Delete/{id}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Delete(int id)
        {
            return Ok(await Mediator.Send(new DeletePriceListCommand { Id = id }));
        }

        [HttpGet("GetItems/{priceListId}")]
        [Authorize(Roles = "SuperAdmin, Manager, Admin, CommunityManager")]
        public async Task<IActionResult> GetItems(int priceListId, [FromQuery] RequestParameter filter)
        {
            return Ok(await Mediator.Send(new GetPriceListItemsQuery
            {
                PriceListId = priceListId,
                Parameter = filter.Parameter,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize
            }));
        }

        [HttpPost("SetItem")]
        [Authorize(Roles = "SuperAdmin, Manager, Admin")]
        public async Task<IActionResult> SetItem([FromBody] SetPriceListItemCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        [HttpPost("BulkSetItems")]
        [Authorize(Roles = "SuperAdmin, Manager, Admin")]
        public async Task<IActionResult> BulkSetItems([FromBody] BulkSetPriceListItemsCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        [HttpDelete("DeleteItem/{itemId}")]
        [Authorize(Roles = "SuperAdmin, Manager, Admin")]
        public async Task<IActionResult> DeleteItem(int itemId)
        {
            return Ok(await Mediator.Send(new DeletePriceListItemCommand { Id = itemId }));
        }

        [HttpPost("AssignToCustomer")]
        [Authorize(Roles = "SuperAdmin, Manager, Admin")]
        public async Task<IActionResult> AssignToCustomer([FromBody] AssignPriceListToCustomerCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        [HttpPost("AssignToCustomerType")]
        [Authorize(Roles = "SuperAdmin, Manager, Admin")]
        public async Task<IActionResult> AssignToCustomerType([FromBody] AssignPriceListToCustomerTypeCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        [HttpGet("GetProductPriceMatrix/{productId}")]
        [Authorize(Roles = "SuperAdmin, Manager, Admin, CommunityManager")]
        public async Task<IActionResult> GetProductPriceMatrix(int productId)
        {
            return Ok(await Mediator.Send(new GetProductPriceMatrixQuery { ProductId = productId }));
        }

        [HttpGet("GetMissingPrices/{priceListId}")]
        [Authorize(Roles = "SuperAdmin, Manager, Admin, CommunityManager")]
        public async Task<IActionResult> GetMissingPrices(int priceListId)
        {
            return Ok(await Mediator.Send(new GetMissingPricesQuery { PriceListId = priceListId }));
        }

        [HttpPost("Clone")]
        [Authorize(Roles = "SuperAdmin, Manager, Admin")]
        public async Task<IActionResult> Clone([FromBody] ClonePriceListCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        [HttpPost("RegenerateFromCost")]
        [Authorize(Roles = "SuperAdmin, Manager, Admin")]
        public async Task<IActionResult> RegenerateFromCost([FromBody] RegeneratePriceListFromCostCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        [HttpPost("SetDefault/{id}")]
        [Authorize(Roles = "SuperAdmin, Manager, Admin")]
        public async Task<IActionResult> SetDefault(int id)
        {
            return Ok(await Mediator.Send(new SetDefaultPriceListCommand { Id = id }));
        }

        [HttpGet("GetProductsForList/{priceListId}")]
        [Authorize(Roles = "SuperAdmin, Manager, Admin, CommunityManager")]
        public async Task<IActionResult> GetProductsForList(int priceListId)
        {
            return Ok(await Mediator.Send(new GetProductsForPriceListQuery { PriceListId = priceListId }));
        }

        [HttpGet("GetEffectiveForCustomer/{customerId}")]
        [Authorize(Roles = "SuperAdmin, Manager, Admin, CommunityManager")]
        public async Task<IActionResult> GetEffectiveForCustomer(Guid customerId)
        {
            return Ok(await Mediator.Send(new GetEffectivePriceListByCustomerIdQuery { CustomerId = customerId }));
        }

        [HttpPost("ResolveForCustomer")]
        [Authorize(Roles = "SuperAdmin, Manager, Admin, CommunityManager, SalesAdvisor")]
        public async Task<IActionResult> ResolveForCustomer([FromBody] ResolveProductPricesForCustomerQuery query)
        {
            return Ok(await Mediator.Send(query));
        }
    }
}
