using MediatR;
using Microsoft.AspNetCore.OutputCaching;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.PriceListSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.PriceListFeature.Commands.BulkSetPriceListItemsCommand
{
    public class BulkSetPriceListItemsCommand : IRequest<Response<int>>
    {
        public int PriceListId { get; set; }
        public List<BulkPriceItem> Items { get; set; } = new();
    }

    public class BulkPriceItem
    {
        public int ProductId { get; set; }
        public decimal Price { get; set; }
    }

    public class BulkSetPriceListItemsCommandHandler : IRequestHandler<BulkSetPriceListItemsCommand, Response<int>>
    {
        private readonly IRepositoryAsync<PriceListItem> _itemRepo;
        private readonly IReadRepositoryAsync<PriceList> _listRepo;
        private readonly IOutputCacheStore _outputCacheStored;

        public BulkSetPriceListItemsCommandHandler(
            IRepositoryAsync<PriceListItem> itemRepo,
            IReadRepositoryAsync<PriceList> listRepo,
            IOutputCacheStore outputCacheStored)
        {
            _itemRepo = itemRepo;
            _listRepo = listRepo;
            _outputCacheStored = outputCacheStored;
        }

        public async Task<Response<int>> Handle(BulkSetPriceListItemsCommand request, CancellationToken cancellationToken)
        {
            var list = await _listRepo.GetByIdAsync(request.PriceListId, cancellationToken);
            if (list == null) throw new KeyNotFoundException($"Lista de precios {request.PriceListId} no encontrada");

            if (request.Items.Any(x => x.Price <= 0))
                throw new ApiException("Todos los precios deben ser mayores a 0.");

            var affected = 0;
            foreach (var entry in request.Items)
            {
                var existing = await _itemRepo.FirstOrDefaultAsync(
                    new PriceListItemByListAndProductSpecification(request.PriceListId, entry.ProductId), cancellationToken);
                if (existing == null)
                {
                    await _itemRepo.AddAsync(new PriceListItem
                    {
                        PriceListId = request.PriceListId,
                        ProductId = entry.ProductId,
                        Price = entry.Price,
                        CreationDate = DateTime.UtcNow,
                        CreatedBy = "admin"
                    }, cancellationToken);
                }
                else
                {
                    existing.Price = entry.Price;
                    existing.ModificationDate = DateTime.UtcNow;
                    existing.ModificatedBy = "admin";
                    await _itemRepo.UpdateAsync(existing, cancellationToken);
                }
                affected++;
            }
            await _itemRepo.SaveChangesAsync(cancellationToken);

            await _outputCacheStored.EvictByTagAsync("cache_pricelists", cancellationToken);
            await _outputCacheStored.EvictByTagAsync("cache_products", cancellationToken);
            await _outputCacheStored.EvictByTagAsync("cache_productsEcommerce", cancellationToken);
            return new Response<int>(affected, $"{affected} precios procesados correctamente");
        }
    }
}
