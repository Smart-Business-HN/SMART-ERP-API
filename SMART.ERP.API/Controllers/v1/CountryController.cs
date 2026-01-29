using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMART.ERP.Application.Features.CountryFeature.Commands.CreateCountryCommand;
using SMART.ERP.Application.Features.CountryFeature.Commands.DeleteCountryCommand;
using SMART.ERP.Application.Features.CountryFeature.Commands.UpdateCountryCommand;
using SMART.ERP.Application.Features.CountryFeature.Queries;
using SMART.ERP.Application.Parameters;

namespace SMART.ERP.API.Controllers.v1
{
    [ApiVersion("1.0")]
    public class CountryController : BaseApiController
    {
        [HttpGet("GetById/{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(int id)
        {
            return Ok(await Mediator.Send(new GetCountryByIdQuery { Id = id }));
        }

        [HttpGet("GetAll")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll([FromQuery] RequestParameter filter, [FromQuery] bool includeCities)
        {
            return Ok(await Mediator.Send(new GetAllCountriesQuery
            {
                Parameter = filter.Parameter,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                Order = filter.Order,
                Column = filter.Column,
                All = filter.All,
                IncludeCities = includeCities
            }));
        }
        [HttpGet("GetAllFromHN")]
        [Authorize]
        public async Task<IActionResult> GetAllFromHN([FromQuery] RequestParameter filter)
        {
            return Ok(await Mediator.Send(new GetAllCountriesFromHNQuery()
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
        public async Task<IActionResult> Create([FromBody] CreateCountryCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        [HttpPut("Update/{id}")]
        [Authorize(Roles = "SuperAdmin, Manager")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCountryCommand command)
        {
            if (id != command.Id)
                return BadRequest(new { message = "Ocurrio un problema con el id de este regitro" });
            return Ok(await Mediator.Send(command));
        }

        [HttpDelete("Delete/{id}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Delete(int id)
        {
            return Ok(await Mediator.Send(new DeleteCountryCommand { Id = id }));
        }
    }
}
