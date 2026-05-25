using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Product;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.ProductCompositionService;
using SMART.ERP.Application.Specifications.ProductSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Domain.Enums;

namespace SMART.ERP.Application.Features.BaseProductFeature.Queries
{
    public class GetAllBaseProductsQuery : IRequest<PagedResponse<List<ProductDto>>>
    {
        public string? Parameter { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? Order { get; set; }
        public string? Column { get; set; }
        public bool All { get; set; }
        public class GetAllBaseProductsQueryHandler : IRequestHandler<GetAllBaseProductsQuery, PagedResponse<List<ProductDto>>>
        {
            private readonly IMapper _mapper;
            private readonly IRepositoryAsync<Product> _repositoryAsync;
            private readonly IRepositoryAsync<Category> _categoryRepositoryAsync;
            private readonly IProductCompositionService _compositionService;

            public GetAllBaseProductsQueryHandler(IMapper mapper, IRepositoryAsync<Product> repositoryAsync,
                IRepositoryAsync<Category> categoryRepositoryAsync,
                IProductCompositionService compositionService)
            {
                _mapper = mapper;
                _repositoryAsync = repositoryAsync;
                _categoryRepositoryAsync = categoryRepositoryAsync;
                _compositionService = compositionService;
            }
            public async Task<PagedResponse<List<ProductDto>>> Handle(GetAllBaseProductsQuery request, CancellationToken cancellationToken)
            {
                if (request.All)
                {
                    request.PageNumber = 0;
                    request.PageSize = await _repositoryAsync.CountAsync();
                }
                var categories = await _categoryRepositoryAsync.ListAsync();
                var products = await _repositoryAsync.ListAsync( new FilterAndPaginationProductSpecification(request.Parameter, request.PageNumber, request.PageSize, request.Order, request.Column));
                var dto = _mapper.Map<List<ProductDto>>(products);
                foreach (var product in dto)
                {
                    product.SubCategory!.Category = _mapper.Map<CategoryDto>(categories.Find(y => y.Id == product.SubCategory.CategoryId));
                }
                var comboIds = dto.Where(d => d.ProductType == ProductType.Combo).Select(d => d.Id).ToList();
                if (comboIds.Count > 0)
                {
                    var stockMap = await _compositionService.GetCalculatedStockMapAsync(comboIds, cancellationToken);
                    foreach (var d in dto)
                    {
                        if (d.ProductType == ProductType.Combo && stockMap.TryGetValue(d.Id, out var stock))
                            d.CalculatedStock = stock;
                    }
                }
                return new PagedResponse<List<ProductDto>>(dto, request.PageNumber, request.PageSize, request.All ? request.PageSize : await _repositoryAsync.CountAsync());
            }
        }
    }
}
