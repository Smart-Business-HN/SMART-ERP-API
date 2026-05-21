using Ardalis.Specification;
using MediatR;
using SMART.ERP.Application.DTOs.PriceList;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.PriceListSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.PriceListFeature.Queries
{
    public class GetProductsForPriceListQuery : IRequest<Response<List<PriceListProductRowDto>>>
    {
        public int PriceListId { get; set; }

        public class GetProductsForPriceListQueryHandler : IRequestHandler<GetProductsForPriceListQuery, Response<List<PriceListProductRowDto>>>
        {
            private readonly IReadRepositoryAsync<Product> _productRepo;
            private readonly IReadRepositoryAsync<PriceList> _listRepo;
            private readonly IReadRepositoryAsync<PriceListItem> _itemRepo;

            public GetProductsForPriceListQueryHandler(
                IReadRepositoryAsync<Product> productRepo,
                IReadRepositoryAsync<PriceList> listRepo,
                IReadRepositoryAsync<PriceListItem> itemRepo)
            {
                _productRepo = productRepo;
                _listRepo = listRepo;
                _itemRepo = itemRepo;
            }

            public async Task<Response<List<PriceListProductRowDto>>> Handle(GetProductsForPriceListQuery request, CancellationToken cancellationToken)
            {
                var list = await _listRepo.GetByIdAsync(request.PriceListId, cancellationToken);
                if (list == null) throw new KeyNotFoundException($"Lista de precios {request.PriceListId} no encontrada");

                var products = await _productRepo.ListAsync(new ActiveProductsSpec(), cancellationToken);
                var items = await _itemRepo.ListAsync(new PriceListItemsByListSpecification(request.PriceListId, null, 0, int.MaxValue), cancellationToken);
                var itemMap = items.ToDictionary(x => x.ProductId);

                var rows = products
                    .OrderBy(p => p.Name)
                    .Select(p =>
                    {
                        var hasItem = itemMap.TryGetValue(p.Id, out var item);
                        return new PriceListProductRowDto
                        {
                            ProductId = p.Id,
                            ProductCode = p.Code,
                            ProductName = p.Name,
                            CostPrice = p.CostPrice,
                            RecomendedSalePrice = p.RecomendedSalePrice,
                            Price = hasItem ? item!.Price : null,
                            HasCustomPrice = hasItem,
                            PriceListItemId = hasItem ? item!.Id : null
                        };
                    })
                    .ToList();

                return new Response<List<PriceListProductRowDto>>(rows);
            }

            private sealed class ActiveProductsSpec : Specification<Product>
            {
                public ActiveProductsSpec()
                {
                    Query.Where(x => x.IsActive).AsNoTracking();
                }
            }
        }
    }
}
