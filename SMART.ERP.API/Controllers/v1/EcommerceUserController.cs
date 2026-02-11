using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMART.ERP.Application.Features.EcommerceUserFeature.Commands.AdminCreateEcommerceUserCommand;
using SMART.ERP.Application.Features.EcommerceUserFeature.Commands.AdminUpdateEcommerceUserCommand;
using SMART.ERP.Application.Features.EcommerceUserFeature.Commands.AdminResetEcommerceUserPasswordCommand;
using SMART.ERP.Application.Features.EcommerceUserFeature.Queries.GetAllEcommerceUsersQuery;
using SMART.ERP.Application.Features.EcommerceUserFeature.Queries.GetEcommerceUserByIdQuery;
using SMART.ERP.Application.Parameters;

namespace SMART.ERP.API.Controllers.v1
{
    [ApiVersion("1.0")]
    public class EcommerceUserController : BaseApiController
    {
        [HttpPost("Create")]
        [Authorize(Roles = "SuperAdmin, Manager")]
        public async Task<IActionResult> Create([FromBody] AdminCreateEcommerceUserCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        [HttpPut("Update/{id}")]
        [Authorize(Roles = "SuperAdmin, Manager")]
        public async Task<IActionResult> Update(Guid id, [FromBody] AdminUpdateEcommerceUserCommand command)
        {
            if (id != command.Id)
                return BadRequest(new { message = "Ocurrio un problema con el id de este registro" });
            return Ok(await Mediator.Send(command));
        }

        [HttpPut("ResetPassword/{id}")]
        [Authorize(Roles = "SuperAdmin, Manager")]
        public async Task<IActionResult> ResetPassword(Guid id, [FromBody] AdminResetEcommerceUserPasswordCommand command)
        {
            if (id != command.Id)
                return BadRequest(new { message = "Ocurrio un problema con el id de este registro" });
            return Ok(await Mediator.Send(command));
        }

        [HttpGet("GetAll")]
        [Authorize(Roles = "SuperAdmin, Manager")]
        public async Task<IActionResult> GetAll([FromQuery] RequestParameter filter,
            [FromQuery] bool? isActive,
            [FromQuery] int? customerTypeId,
            [FromQuery] int? departmentId,
            [FromQuery] DateTime? dateFrom,
            [FromQuery] DateTime? dateTo)
        {
            return Ok(await Mediator.Send(new GetAllEcommerceUsersQuery
            {
                Parameter = filter.Parameter,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                Order = filter.Order,
                Column = filter.Column,
                All = filter.All,
                IsActive = isActive,
                CustomerTypeId = customerTypeId,
                DepartmentId = departmentId,
                DateFrom = dateFrom,
                DateTo = dateTo
            }));
        }

        [HttpGet("GetById/{id}")]
        [Authorize(Roles = "SuperAdmin, Manager")]
        public async Task<IActionResult> GetById(Guid id)
        {
            return Ok(await Mediator.Send(new GetEcommerceUserByIdQuery { Id = id }));
        }
    }
}
