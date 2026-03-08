using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMART.ERP.Application.Features.QuotationFeature.Commands.CopyQuotationFromIdCommand;
using SMART.ERP.Application.Features.QuotationFeature.Commands.CreateQuotationCommand;
using SMART.ERP.Application.Features.QuotationFeature.Commands.RestoreQuotationFromSnapshotCommand;
using SMART.ERP.Application.Features.QuotationFeature.Commands.UpdateQuotationCommand;
using SMART.ERP.Application.Features.QuotationFeature.Queries;
using SMART.ERP.Application.Features.QuotationPreviewFeature.Commands.CreateQuotationAdminCommentCommand;
using SMART.ERP.Application.Features.QuotationPreviewFeature.Commands.GenerateQuotationAccessTokenCommand;
using SMART.ERP.Application.Features.QuotationPreviewFeature.Queries;
using SMART.ERP.Application.Parameters;

namespace SMART.ERP.API.Controllers.v1
{
    [ApiVersion("1.0")]
    public class QuotationController : BaseApiController
    {
        [HttpGet("GetById/{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(int id)
        {
            return Ok(await Mediator.Send(new GetQuotationByIdQuery { Id = id }));
        }
        [HttpPost("Create")]
        [Authorize(Roles = "SuperAdmin, Admin, Manager, SalesAdvisor")]
        public async Task<IActionResult> Create([FromBody] CreateQuotationCommand command)
        {
            return Ok(await Mediator.Send(command));
        }
        [HttpPost("CopyQuotationFromId/{id}")]
        [Authorize(Roles = "SuperAdmin, Admin, Manager, SalesAdvisor")]
        public async Task<IActionResult> CopyQuotationFromId(int id,[FromBody] CopyQuotationFromIdCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest("Ocurrio un error con el id de este registro");
            }
            return Ok(await Mediator.Send(command));
        }
        [HttpGet("GetAll")]
        [Authorize]
        public async Task<IActionResult> GetAll([FromQuery] RequestParameter filter)
        {
            return Ok(await Mediator.Send(new GetAllQuotationQuery
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
        [Authorize(Roles = "SuperAdmin, Admin, Manager, CommunityManager, SalesAdvisor")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateQuotationCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest("Ocurrio un error con el id de este registro");
            }
            return Ok(await Mediator.Send(command));
        }

        [HttpGet("GetVersions/{quotationId}")]
        [Authorize]
        public async Task<IActionResult> GetVersions(int quotationId, [FromQuery] RequestParameter filter)
        {
            return Ok(await Mediator.Send(new GetQuotationVersionsQuery
            {
                QuotationId = quotationId,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize
            }));
        }

        [HttpGet("GetSnapshot/{id}")]
        [Authorize]
        public async Task<IActionResult> GetSnapshot(int id)
        {
            return Ok(await Mediator.Send(new GetQuotationSnapshotByIdQuery { Id = id }));
        }

        [HttpPost("RestoreFromSnapshot/{quotationId}")]
        [Authorize(Roles = "SuperAdmin, Admin, Manager")]
        public async Task<IActionResult> RestoreFromSnapshot(int quotationId, [FromBody] RestoreQuotationFromSnapshotCommand command)
        {
            if (quotationId != command.QuotationId)
            {
                return BadRequest("Ocurrio un error con el id de este registro");
            }
            return Ok(await Mediator.Send(command));
        }

        [HttpPost("GenerateAccessToken/{id}")]
        [Authorize(Roles = "SuperAdmin, Admin, Manager, SalesAdvisor")]
        public async Task<IActionResult> GenerateAccessToken(int id)
        {
            return Ok(await Mediator.Send(new GenerateQuotationAccessTokenCommand { QuotationId = id }));
        }

        [HttpGet("GetComments/{quotationId}")]
        [Authorize]
        public async Task<IActionResult> GetComments(int quotationId)
        {
            return Ok(await Mediator.Send(new GetQuotationCommentsByQuotationIdQuery { QuotationId = quotationId }));
        }

        [HttpPost("AddAdminComment")]
        [Authorize]
        public async Task<IActionResult> AddAdminComment([FromBody] CreateQuotationAdminCommentCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        [HttpGet("GetItemObservations/{quotationId}")]
        [Authorize]
        public async Task<IActionResult> GetItemObservations(int quotationId)
        {
            return Ok(await Mediator.Send(new GetQuotationItemObservationsByQuotationIdQuery { QuotationId = quotationId }));
        }

    }
}
