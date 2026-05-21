using Ardalis.Specification;
using MediatR;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.PriceListSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.PriceListFeature.Queries
{
    public class GetMissingPricesQuery : IRequest<Response<List<MissingPriceDto>>>
    {
        public int PriceListId { get; set; }

        public class GetMissingPricesQueryHandler : IRequestHandler<GetMissingPricesQuery, Response<List<MissingPriceDto>>>
        {
            private readonly IReadRepositoryAsync<Product> _productRepo;
            private readonly IReadRepositoryAsync<PriceListItem> _itemRepo;

            public GetMissingPricesQueryHandler(IReadRepositoryAsync<Product> productRepo, IReadRepositoryAsync<PriceListItem> itemRepo)
            {
                _productRepo = productRepo;
                _itemRepo = itemRepo;
            }

            public async Task<Response<List<MissingPriceDto>>> Handle(GetMissingPricesQuery request, CancellationToken cancellationToken)
            {
                var products = await _productRepo.ListAsync(new ActiveProductsSpec(), cancellationToken);
                var items = await _itemRepo.ListAsync(new PriceListItemsByListSpecification(request.PriceListId, null, 0, int.MaxValue), cancellationToken);
                var productIdsWithPrice = items.Select(x => x.ProductId).ToHashSet();

                var missing = products
                    .Where(p => !productIdsWithPrice.Contains(p.Id))
                    .Select(p => new MissingPriceDto
                    {
                        ProductId = p.Id,
                        ProductCode = p.Code,
                        ProductName = p.Name,
                        CostPrice = p.CostPrice
                    })
                    .ToList();

                return new Response<List<MissingPriceDto>>(missing, $"{missing.Count} productos sin precio en esta lista");
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

    public class MissingPriceDto
    {
        public int ProductId { get; set; }
        public string? ProductCode { get; set; }
        public string? ProductName { get; set; }
        public decimal CostPrice { get; set; }
    }
}
