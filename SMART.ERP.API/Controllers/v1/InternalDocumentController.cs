using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMART.ERP.Application.Features.BrandFeature.Commands.CreateBrandCommand;
using SMART.ERP.Application.Features.BrandFeature.Queries;
using SMART.ERP.Application.Features.InternalDocumentFeature.Queries;
using SMART.ERP.Application.Parameters;

namespace SMART.ERP.API.Controllers.v1
{
    [ApiVersion("1.0")]
    public class InternalDocumentController : BaseApiController
    {
        [HttpGet("GetAll")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll([FromQuery] RequestParameter filter)
        {

            return Ok(await Mediator.Send(new GetAllInternalDocumentsQuery
            {
                Parameter = filter.Parameter,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                Order = filter.Order,
                Column = filter.Column,
                All = filter.All
            }));
        }
    }
}
