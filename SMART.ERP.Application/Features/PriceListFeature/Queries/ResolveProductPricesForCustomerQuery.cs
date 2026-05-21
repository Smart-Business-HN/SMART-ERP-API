using MediatR;
using SMART.ERP.Application.DTOs.PriceList;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.PriceListResolver;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.PriceListFeature.Queries
{
    public class ResolveProductPricesForCustomerQuery : IRequest<Response<ResolvedPricesBatchDto>>
    {
        public Guid? CustomerId { get; set; }
        public List<int> ProductIds { get; set; } = new();

        public class ResolveProductPricesForCustomerQueryHandler : IRequestHandler<ResolveProductPricesForCustomerQuery, Response<ResolvedPricesBatchDto>>
        {
            private readonly IPriceListService _priceListService;
            private readonly IReadRepositoryAsync<PriceList> _priceListRepo;

            public ResolveProductPricesForCustomerQueryHandler(
                IPriceListService priceListService,
                IReadRepositoryAsync<PriceList> priceListRepo)
            {
                _priceListService = priceListService;
                _priceListRepo = priceListRepo;
            }

            public async Task<Response<ResolvedPricesBatchDto>> Handle(ResolveProductPricesForCustomerQuery request, CancellationToken cancellationToken)
            {
                var listId = await _priceListService.ResolvePriceListIdAsync(customerId: request.CustomerId, ct: cancellationToken);
                var prices = await _priceListService.GetPricesAsync(request.ProductIds, listId, cancellationToken);

                PriceList? list = listId.HasValue
                    ? await _priceListRepo.GetByIdAsync(listId.Value, cancellationToken)
                    : null;

                var dto = new ResolvedPricesBatchDto
                {
                    PriceListId = listId,
                    PriceListName = list?.Name,
                    IsDefault = list?.IsDefault ?? false,
                    Items = request.ProductIds.Distinct().Select(productId =>
                    {
                        var has = prices.TryGetValue(productId, out var price) && price > 0;
                        return new ResolvedBatchItemDto
                        {
                            ProductId = productId,
                            Price = has ? price : null,
                            HasPrice = has
                        };
                    }).ToList()
                };

                return new Response<ResolvedPricesBatchDto>(dto);
            }
        }
    }
}
