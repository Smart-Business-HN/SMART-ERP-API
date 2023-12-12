using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using SMART.ERP.Application.Features.BaseProductFeature.Queries;
using SMART.ERP.Application.Parameters;
using SMART.ERP.Application.Services.HeaderService;

namespace SMART.ERP.API.Controllers.v2
{
    [ApiVersion("2.0")]
    public class ProductController : BaseApiController
    {
        private readonly IHeaderService _headerService;

        public ProductController(IHeaderService headerService)
        {
            _headerService = headerService;
        }

        [HttpGet("GetBySlug/{slug}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetBySlug(string slug)
        {
            return Ok(await Mediator.Send(new GetBaseProductBySlugQuery { Slug = slug }));
        }
        [HttpGet("GetAll")]
        [AllowAnonymous]
        [OutputCache (PolicyName = "cache_productsEcommerce")]
        public async Task<IActionResult> GetAll([FromQuery] RequestParameter filter)
        {
            return Ok(await Mediator.Send(new GetAllProductForEcommerceQuery()
            {
                Parameter = filter.Parameter,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                Order = filter.Order,
                Column = filter.Column,
                All = filter.All
            }));
        }
        [HttpGet("GetProductsBySameCategorySlug/{categorySlug}/{productSlug}")]
        [AllowAnonymous]
        [OutputCache (PolicyName = "cache_productsBySameCategorySlug")]
        public async Task<IActionResult> GetProductsBySameCategorySlug(string categorySlug, string productSlug)
        {
            return Ok(await Mediator.Send(new GetProductsBySameCategorySlugQuery { CategorySlug = categorySlug, ProductSlug = productSlug }
            ));
        }
        [HttpGet("GetProductsBySameSubCategorySlug/{subCategorySlug}/{productSlug}")]
        [AllowAnonymous]
        [OutputCache(PolicyName = "cache_productsBySameSubCategorySlug")]
        public async Task<IActionResult> GetProductsBySameSubCategorySlug(string subCategorySlug, string productSlug)
        {
            return Ok(await Mediator.Send(new GetProductsBySameSubCategorySlugQuery { SubCategorySlug = subCategorySlug, ProductSlug = productSlug }
            ));
        }
        [HttpGet("GetProducsByCategorySlug/{categorySlug}")]
        [AllowAnonymous]
        [OutputCache(PolicyName = "cache_producsByCategorySlug")]
        public async Task<IActionResult> GetProductsByCategorySlug(string categorySlug,[FromQuery] RequestParameter filter)
        {
            return Ok(await Mediator.Send(new GetAllBaseProductsByCategorySlugQuery()
            {
                CategorySlug = categorySlug,
                Parameter = filter.Parameter,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                Order = filter.Order,
                Column = filter.Column
            }));
        }
        [HttpGet("GetProductsBySubCategorySlug/{subCategorySlug}")]
        [AllowAnonymous]
        [OutputCache(PolicyName = "cache_productsBySubCategorySlug")]
        public async Task<IActionResult> GetProductsBySubCategorySlug(string subCategorySlug, [FromQuery] RequestParameter filter)
        {
            return Ok(await Mediator.Send(new GetAllBaseProductsBySubCategorySlugQuery()
            {
                SubCategorySlug = subCategorySlug,
                Parameter = filter.Parameter,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                Order = filter.Order,
                Column = filter.Column
            }));
        }

    }
}
