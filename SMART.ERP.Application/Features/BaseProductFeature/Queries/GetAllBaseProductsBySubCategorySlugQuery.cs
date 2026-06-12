using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Product;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.InventoryDistributionSpecification;
using SMART.ERP.Application.Specifications.ProductSpecification;
using SMART.ERP.Application.Specifications.SubcategorySpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.BaseProductFeature.Queries
{
    public class GetAllBaseProductsBySubCategorySlugQuery : IRequest<PagedResponse<List<ProductDto>>>
    {
        public string SubCategorySlug { get; set; } = null!;
        public string? Parameter { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? Order { get; set; }
        public string? Column { get; set; }
        public bool? IsUserSignIn { get; set; }
        public int? CustomerTypeId { get; set; }
        public class GetAllBaseProductsBySubCategorySlugQueryHandler : IRequestHandler<GetAllBaseProductsBySubCategorySlugQuery, PagedResponse<List<ProductDto>>>
        {
            private readonly IMapper _mapper;
            private readonly IRepositoryAsync<Product> _repositoryAsync;
            private readonly IRepositoryAsync<Subcategory> _subCategoryRepositoryAsync;
            private readonly IRepositoryAsync<InventoryDistribution> _inventoryRepositoryAsync;

            public GetAllBaseProductsBySubCategorySlugQueryHandler(IMapper mapper, IRepositoryAsync<Product> repositoryAsync,
                IRepositoryAsync<Subcategory> subCategoryRepositoryAsync, IRepositoryAsync<InventoryDistribution> inventoryRepositoryAsync)
            {
                _mapper = mapper;
                _repositoryAsync = repositoryAsync;
                _subCategoryRepositoryAsync = subCategoryRepositoryAsync;
                _inventoryRepositoryAsync = inventoryRepositoryAsync;
            }
            public async Task<PagedResponse<List<ProductDto>>> Handle(GetAllBaseProductsBySubCategorySlugQuery request, CancellationToken cancellationToken)
            {
                var checkCategory = await _subCategoryRepositoryAsync.FirstOrDefaultAsync(new GetSubCategoryBySlugSpecification(request.SubCategorySlug));
                if (checkCategory == null)
                {
                    throw new KeyNotFoundException($"Registro no encontrado con el slug {request.SubCategorySlug}");
                }
                var products = await _repositoryAsync.ListAsync(
                    new FilterAndPaginationProductsBySubCategorySlugSpecification(request.SubCategorySlug, request.Parameter, request.PageNumber - 1, request.PageSize, request.Order, request.Column));
                if (request.IsUserSignIn.HasValue && request.IsUserSignIn.Value)
                {
                    foreach (var item in products)
                    {
                        item.RecomendedSalePrice = Math.Ceiling((item.CostPrice * (decimal)1.2) * (1 + (item.Tax!.Rate / 100)));
                        item.CostPrice = 0;
                        item.Tax = null;
                    }
                }
                else
                {
                    foreach (var item in products)
                    {
                        item.RecomendedSalePrice = Math.Ceiling((item.CostPrice * (decimal)1.3) * (1 + (item.Tax!.Rate / 100)));
                        item.CostPrice = 0;
                        item.Tax = null;
                    }
                }
                var dto = _mapper.Map<List<ProductDto>>(products);
                // Disponibilidad ecommerce (físico + virtual) sin tocar CurrentStock.
                var distributions = await _inventoryRepositoryAsync.ListAsync(new FilterInventoryByProductIdsSpec(dto.Select(d => d.Id).ToList()));
                ProductAvailabilityHelper.ApplyEcommerceStock(dto, distributions);
                return new PagedResponse<List<ProductDto>>(dto, request.PageNumber, request.PageSize, false ? request.PageSize : await _repositoryAsync.CountAsync(new FilterAndPaginationProductsBySubCategorySlugSpecification(request.SubCategorySlug, request.Parameter, 0, 0, request.Order, request.Column)));
            }
        }
    }
}
