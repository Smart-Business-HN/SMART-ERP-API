using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using SMART.ERP.Application.Features.CategoryFeature.Queries;

namespace SMART.ERP.API.Controllers.v2
{
    [ApiVersion("2.0")]
    public class CategoryController : BaseApiController
    {
        [HttpGet("GetAllNavCategory")]
        [AllowAnonymous]
        [OutputCache (PolicyName = "cache_getAllNavCategories")]
        public async Task<IActionResult> GetAllNavCategory()
        {
            //if (!_headerService.VerificatedSecretKey())
            //    return Unauthorized();

            return Ok(await Mediator.Send(new GetAllNavCategoriesQuery()));
        }
    }
}
