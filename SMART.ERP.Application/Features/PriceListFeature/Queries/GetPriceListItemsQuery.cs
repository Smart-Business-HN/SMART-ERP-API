using MediatR;
using SMART.ERP.Application.DTOs.PriceList;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.PriceListSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.PriceListFeature.Queries
{
    public class GetPriceListItemsQuery : IRequest<PagedResponse<List<PriceListItemDto>>>
    {
        public int PriceListId { get; set; }
        public string? Parameter { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }

        public class GetPriceListItemsQueryHandler : IRequestHandler<GetPriceListItemsQuery, PagedResponse<List<PriceListItemDto>>>
        {
            private readonly IReadRepositoryAsync<PriceListItem> _itemRepo;

            public GetPriceListItemsQueryHandler(IReadRepositoryAsync<PriceListItem> itemRepo)
            {
                _itemRepo = itemRepo;
            }

            public async Task<PagedResponse<List<PriceListItemDto>>> Handle(GetPriceListItemsQuery request, CancellationToken cancellationToken)
            {
                var items = await _itemRepo.ListAsync(
                    new PriceListItemsByListSpecification(request.PriceListId, request.Parameter, request.PageNumber, request.PageSize),
                    cancellationToken);

                var dtos = items.Select(x => new PriceListItemDto
                {
                    Id = x.Id,
                    PriceListId = x.PriceListId,
                    ProductId = x.ProductId,
                    ProductCode = x.Product?.Code,
                    ProductName = x.Product?.Name,
                    Price = x.Price,
                    CostPrice = x.Product?.CostPrice ?? 0m,
                    MarginPct = (x.Product?.CostPrice ?? 0m) > 0
                        ? Math.Round(((x.Price / x.Product!.CostPrice) - 1m) * 100m, 2)
                        : 0m
                }).ToList();

                var total = await _itemRepo.CountAsync(
                    new CountPriceListItemsByListSpecification(request.PriceListId, request.Parameter), cancellationToken);

                return new PagedResponse<List<PriceListItemDto>>(dtos, request.PageNumber, request.PageSize, total);
            }
        }
    }
}
