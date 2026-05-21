using MediatR;
using SMART.ERP.Application.DTOs.PriceList;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.PriceListSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.PriceListFeature.Queries
{
    public class GetProductPriceMatrixQuery : IRequest<Response<ProductPriceMatrixDto>>
    {
        public int ProductId { get; set; }

        public class GetProductPriceMatrixQueryHandler : IRequestHandler<GetProductPriceMatrixQuery, Response<ProductPriceMatrixDto>>
        {
            private readonly IReadRepositoryAsync<Product> _productRepo;
            private readonly IReadRepositoryAsync<PriceList> _listRepo;
            private readonly IReadRepositoryAsync<PriceListItem> _itemRepo;

            public GetProductPriceMatrixQueryHandler(
                IReadRepositoryAsync<Product> productRepo,
                IReadRepositoryAsync<PriceList> listRepo,
                IReadRepositoryAsync<PriceListItem> itemRepo)
            {
                _productRepo = productRepo;
                _listRepo = listRepo;
                _itemRepo = itemRepo;
            }

            public async Task<Response<ProductPriceMatrixDto>> Handle(GetProductPriceMatrixQuery request, CancellationToken cancellationToken)
            {
                var product = await _productRepo.GetByIdAsync(request.ProductId, cancellationToken);
                if (product == null) throw new KeyNotFoundException($"Producto {request.ProductId} no encontrado");

                var lists = await _listRepo.ListAsync(cancellationToken);
                var items = await _itemRepo.ListAsync(new PriceListItemsByProductSpecification(request.ProductId), cancellationToken);
                var itemMap = items.ToDictionary(x => x.PriceListId);

                var dto = new ProductPriceMatrixDto
                {
                    ProductId = product.Id,
                    ProductCode = product.Code,
                    ProductName = product.Name,
                    CostPrice = product.CostPrice,
                    PriceLists = lists.OrderByDescending(x => x.IsDefault).ThenBy(x => x.Name).Select(l => new ProductPriceMatrixEntryDto
                    {
                        PriceListId = l.Id,
                        PriceListName = l.Name,
                        IsDefault = l.IsDefault,
                        Price = itemMap.TryGetValue(l.Id, out var it) ? it.Price : null,
                        PriceListItemId = itemMap.TryGetValue(l.Id, out var it2) ? it2.Id : null
                    }).ToList()
                };

                return new Response<ProductPriceMatrixDto>(dto);
            }
        }
    }
}
