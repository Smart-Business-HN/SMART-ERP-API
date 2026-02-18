using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using SMART.ERP.Application.Features.BaseProductFeature.Queries;
using SMART.ERP.Application.Features.ProductDropshippingFeature.Queries;
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
        public async Task<IActionResult> GetBySlug([FromQuery] bool isLogged, [FromQuery] int customerTypeId, string slug)
        {
            return Ok(await Mediator.Send(new GetBaseProductBySlugQuery { Slug = slug, CustomerTypeId = customerTypeId, IsLogged = isLogged }));
        }
        [HttpGet("GetAll")]
        [AllowAnonymous]
        [OutputCache(PolicyName = "cache_productsEcommerce")]
        public async Task<IActionResult> GetAll([FromQuery] RequestEcommerceParameter filter)
        {
            return Ok(await Mediator.Send(new GetAllProductForEcommerceQuery()
            {
                Parameter = filter.Parameter,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                Order = filter.Order,
                Column = filter.Column,
                All = filter.All,
                IsUserSignIn = filter.IsUserSignIn,
                CustomerTypeId = filter.CustomerTypeId
            }));
        }
        [HttpGet("GetProductsBySameCategorySlug/{categorySlug}/{productSlug}")]
        [AllowAnonymous]
        [OutputCache(PolicyName = "cache_productsBySameCategorySlug")]
        public async Task<IActionResult> GetProductsBySameCategorySlug([FromQuery] bool isLogged, [FromQuery] int customerTypeId, string categorySlug, string productSlug)
        {
            return Ok(await Mediator.Send(new GetProductsBySameCategorySlugQuery { CategorySlug = categorySlug, ProductSlug = productSlug, IsLogged = isLogged, CustomerTypeId = customerTypeId }
            ));
        }
        [HttpGet("GetProductsBySameSubCategorySlug/{subCategorySlug}/{productSlug}")]
        [AllowAnonymous]
        [OutputCache(PolicyName = "cache_productsBySameSubCategorySlug")]
        public async Task<IActionResult> GetProductsBySameSubCategorySlug([FromQuery] bool isLogged,[FromQuery] int customerTypeId, string subCategorySlug, string productSlug)
        {
            return Ok(await Mediator.Send(new GetProductsBySameSubCategorySlugQuery { SubCategorySlug = subCategorySlug, ProductSlug = productSlug, CustomerTypeId = customerTypeId, IsLogged = isLogged }
            ));
        }
        [HttpGet("GetProducsByCategorySlug/{categorySlug}")]
        [AllowAnonymous]
        [OutputCache(PolicyName = "cache_productsByCategorySlug")]
        public async Task<IActionResult> GetProductsByCategorySlug(string categorySlug, [FromQuery] RequestEcommerceParameter filter)
        {
            return Ok(await Mediator.Send(new GetAllBaseProductsByCategorySlugQuery()
            {
                CategorySlug = categorySlug,
                Parameter = filter.Parameter,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                Order = filter.Order,
                Column = filter.Column,
                IsUserSignIn = filter.IsUserSignIn,
                CustomerTypeId = filter.CustomerTypeId
            }));
        }
        [HttpGet("GetProductsBySubCategorySlug/{subCategorySlug}")]
        [AllowAnonymous]
        [OutputCache(PolicyName = "cache_productsBySubCategorySlug")]
        public async Task<IActionResult> GetProductsBySubCategorySlug(string subCategorySlug, [FromQuery] RequestEcommerceParameter filter)
        {
            return Ok(await Mediator.Send(new GetAllBaseProductsBySubCategorySlugQuery()
            {
                SubCategorySlug = subCategorySlug,
                Parameter = filter.Parameter,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                Order = filter.Order,
                Column = filter.Column,
                IsUserSignIn = filter.IsUserSignIn,
                CustomerTypeId = filter.CustomerTypeId
            }));
        }

        [HttpGet("Search")]
        [AllowAnonymous]
        [OutputCache(PolicyName = "cache_productSearch", VaryByQueryKeys = new[] { "*" })]
        public async Task<IActionResult> SearchProducts([FromQuery] ProductSearchParameter searchParams)
        {
            return Ok(await Mediator.Send(new SearchProductsQuery { SearchParameters = searchParams }));
        }

        [HttpGet("SearchSuggestions")]
        [AllowAnonymous]
        [OutputCache(PolicyName = "cache_searchSuggestions", VaryByQueryKeys = new[] { "searchTerm", "limit" })]
        public async Task<IActionResult> GetSearchSuggestions([FromQuery] string searchTerm, [FromQuery] int limit = 10)
        {
            return Ok(await Mediator.Send(new GetSearchSuggestionsQuery
            {
                SearchTerm = searchTerm,
                Limit = limit
            }));
        }

        [HttpGet("GetAvailability/{productId}")]
        [Authorize(Roles = "SuperAdmin, Manager, Admin, SalesAdvisor")]
        public async Task<IActionResult> GetAvailability(int productId)
        {
            return Ok(await Mediator.Send(new GetProductAvailabilityQuery { ProductId = productId }));
        }

        [HttpGet("GetPriceWithShipping/{productId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPriceWithShipping(
            int productId,
            [FromQuery] bool isUserSignedIn,
            [FromQuery] int? customerTypeId,
            [FromQuery] int? destinationCityId)
        {
            return Ok(await Mediator.Send(new GetProductPriceWithShippingQuery
            {
                ProductId = productId,
                IsUserSignedIn = isUserSignedIn,
                CustomerTypeId = customerTypeId,
                DestinationCityId = destinationCityId
            }));
        }

    }
}
