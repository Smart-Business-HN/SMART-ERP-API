using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Product;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services;
using SMART.ERP.Application.Specifications.ProductSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.BaseProductFeature.Queries
{
    public class GetProductsBySameSubCategorySlugQuery : IRequest<Response<List<ResumeProductDto>>>
    {
        public string SubCategorySlug { get; set; } = null!;
        public string ProductSlug { get; set; } = null!;
        public bool IsLogged { get; set; } = false;
        public int? CustomerTypeId { get; set; }
    }

    public class GetProductsBySameSubCategorySlugQueryHandler : IRequestHandler<GetProductsBySameSubCategorySlugQuery, Response<List<ResumeProductDto>>>
    {
        private readonly IRepositoryAsync<Product> _repositoryAsync;
        private readonly IProductPricingService _productPricingService;
        private readonly IMapper _mapper;

        public GetProductsBySameSubCategorySlugQueryHandler(IRepositoryAsync<Product> repositoryAsync, IProductPricingService productPricingService, IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _productPricingService = productPricingService;
            _mapper = mapper;
        }

        public async Task<Response<List<ResumeProductDto>>> Handle(GetProductsBySameSubCategorySlugQuery request, CancellationToken cancellationToken)
        {
            var products = await _repositoryAsync.ListAsync(new FilterProductsBySubCategorySlugAndProductSlugSpecification(request.SubCategorySlug, request.ProductSlug));
            if (products == null || products.Count == 0)
            {
                throw new KeyNotFoundException($"No se encontraron productos en la subcategoría con slug {request.SubCategorySlug}");
            }

            var prices = await _productPricingService.CalculateRecommendedSalePricesAsync(
                products.Select(p => p.Id),
                request.IsLogged,
                request.CustomerTypeId,
                ct: cancellationToken);
            foreach (var item in products)
            {
                item.RecomendedSalePrice = prices.GetValueOrDefault(item.Id, 0);
                item.CostPrice = 0;
                item.Tax = null;
            }

            var dto = _mapper.Map<List<ResumeProductDto>>(products);
            var producsToShow = SelectRandomElements(dto, 5);
            return new Response<List<ResumeProductDto>>(producsToShow);
        }
        static List<ResumeProductDto> SelectRandomElements(List<ResumeProductDto> inputArray, int count)
        {
            Random random = new Random();
            List<ResumeProductDto> resultArray = inputArray.OrderBy(_ => random.Next()).Take(count).ToList();

            return resultArray;
        }
    }
}
