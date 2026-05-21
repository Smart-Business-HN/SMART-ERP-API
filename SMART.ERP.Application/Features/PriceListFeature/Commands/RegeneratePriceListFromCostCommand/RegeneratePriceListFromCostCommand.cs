using Ardalis.Specification;
using MediatR;
using Microsoft.AspNetCore.OutputCaching;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.PriceListSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.PriceListFeature.Commands.RegeneratePriceListFromCostCommand
{
    public class RegeneratePriceListFromCostCommand : IRequest<Response<int>>
    {
        public int PriceListId { get; set; }
        public decimal Multiplier { get; set; }
        public bool ApplyTax { get; set; } = true;
    }

    public class RegeneratePriceListFromCostCommandHandler : IRequestHandler<RegeneratePriceListFromCostCommand, Response<int>>
    {
        private readonly IRepositoryAsync<PriceList> _listRepo;
        private readonly IRepositoryAsync<PriceListItem> _itemRepo;
        private readonly IReadRepositoryAsync<Product> _productRepo;
        private readonly IOutputCacheStore _outputCacheStored;

        public RegeneratePriceListFromCostCommandHandler(
            IRepositoryAsync<PriceList> listRepo,
            IRepositoryAsync<PriceListItem> itemRepo,
            IReadRepositoryAsync<Product> productRepo,
            IOutputCacheStore outputCacheStored)
        {
            _listRepo = listRepo;
            _itemRepo = itemRepo;
            _productRepo = productRepo;
            _outputCacheStored = outputCacheStored;
        }

        public async Task<Response<int>> Handle(RegeneratePriceListFromCostCommand request, CancellationToken cancellationToken)
        {
            if (request.Multiplier <= 0) throw new ApiException("Multiplicador debe ser mayor a 0.");
            var list = await _listRepo.GetByIdAsync(request.PriceListId, cancellationToken);
            if (list == null) throw new KeyNotFoundException($"Lista {request.PriceListId} no encontrada");

            var products = await _productRepo.ListAsync(new ActiveProductsWithTaxSpec(), cancellationToken);
            var existing = await _itemRepo.ListAsync(new PriceListItemsByListSpecification(request.PriceListId, null, 0, int.MaxValue), cancellationToken);
            var existingMap = existing.ToDictionary(x => x.ProductId);

            var count = 0;
            foreach (var p in products)
            {
                var taxRate = request.ApplyTax ? (p.Tax?.Rate ?? 0m) : 0m;
                var newPrice = Math.Ceiling((p.CostPrice * request.Multiplier) * (1m + (taxRate / 100m)));
                if (newPrice <= 0) continue;

                if (existingMap.TryGetValue(p.Id, out var item))
                {
                    item.Price = newPrice;
                    item.ModificationDate = DateTime.UtcNow;
                    item.ModificatedBy = "admin";
                    await _itemRepo.UpdateAsync(item, cancellationToken);
                }
                else
                {
                    await _itemRepo.AddAsync(new PriceListItem
                    {
                        PriceListId = request.PriceListId,
                        ProductId = p.Id,
                        Price = newPrice,
                        CreationDate = DateTime.UtcNow,
                        CreatedBy = "admin"
                    }, cancellationToken);
                }
                count++;
            }
            await _itemRepo.SaveChangesAsync(cancellationToken);

            await _outputCacheStored.EvictByTagAsync("cache_pricelists", cancellationToken);
            await _outputCacheStored.EvictByTagAsync("cache_products", cancellationToken);
            await _outputCacheStored.EvictByTagAsync("cache_productsEcommerce", cancellationToken);
            return new Response<int>(count, $"{count} precios regenerados desde costo (×{request.Multiplier})");
        }

        private sealed class ActiveProductsWithTaxSpec : Specification<Product>
        {
            public ActiveProductsWithTaxSpec()
            {
                Query.Where(x => x.IsActive).Include(x => x.Tax);
            }
        }
    }
}
