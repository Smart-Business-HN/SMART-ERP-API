using MediatR;
using Microsoft.AspNetCore.OutputCaching;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.PriceListSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.PriceListFeature.Commands.SetPriceListItemCommand
{
    public class SetPriceListItemCommand : IRequest<Response<PriceListItem>>
    {
        public int PriceListId { get; set; }
        public int ProductId { get; set; }
        public decimal Price { get; set; }
    }

    public class SetPriceListItemCommandHandler : IRequestHandler<SetPriceListItemCommand, Response<PriceListItem>>
    {
        private readonly IRepositoryAsync<PriceListItem> _itemRepo;
        private readonly IReadRepositoryAsync<PriceList> _listRepo;
        private readonly IReadRepositoryAsync<Product> _productRepo;
        private readonly IOutputCacheStore _outputCacheStored;

        public SetPriceListItemCommandHandler(
            IRepositoryAsync<PriceListItem> itemRepo,
            IReadRepositoryAsync<PriceList> listRepo,
            IReadRepositoryAsync<Product> productRepo,
            IOutputCacheStore outputCacheStored)
        {
            _itemRepo = itemRepo;
            _listRepo = listRepo;
            _productRepo = productRepo;
            _outputCacheStored = outputCacheStored;
        }

        public async Task<Response<PriceListItem>> Handle(SetPriceListItemCommand request, CancellationToken cancellationToken)
        {
            var list = await _listRepo.GetByIdAsync(request.PriceListId, cancellationToken);
            if (list == null) throw new KeyNotFoundException($"Lista de precios {request.PriceListId} no encontrada");

            var product = await _productRepo.GetByIdAsync(request.ProductId, cancellationToken);
            if (product == null) throw new KeyNotFoundException($"Producto {request.ProductId} no encontrado");

            if (request.Price <= 0)
                throw new ApiException("El precio debe ser mayor a 0.");

            var existing = await _itemRepo.FirstOrDefaultAsync(
                new PriceListItemByListAndProductSpecification(request.PriceListId, request.ProductId), cancellationToken);

            PriceListItem result;
            if (existing == null)
            {
                var newItem = new PriceListItem
                {
                    PriceListId = request.PriceListId,
                    ProductId = request.ProductId,
                    Price = request.Price,
                    CreationDate = DateTime.UtcNow,
                    CreatedBy = "admin"
                };
                result = await _itemRepo.AddAsync(newItem, cancellationToken);
            }
            else
            {
                existing.Price = request.Price;
                existing.ModificationDate = DateTime.UtcNow;
                existing.ModificatedBy = "admin";
                await _itemRepo.UpdateAsync(existing, cancellationToken);
                result = existing;
            }
            await _itemRepo.SaveChangesAsync(cancellationToken);

            await _outputCacheStored.EvictByTagAsync("cache_pricelists", cancellationToken);
            await _outputCacheStored.EvictByTagAsync("cache_products", cancellationToken);
            await _outputCacheStored.EvictByTagAsync("cache_productsEcommerce", cancellationToken);
            return new Response<PriceListItem>(result, "Precio guardado correctamente");
        }
    }
}
