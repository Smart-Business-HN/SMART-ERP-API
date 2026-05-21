using MediatR;
using Microsoft.AspNetCore.OutputCaching;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.PriceListFeature.Commands.DeletePriceListItemCommand
{
    public class DeletePriceListItemCommand : IRequest<Response<string>>
    {
        public int Id { get; set; }
    }

    public class DeletePriceListItemCommandHandler : IRequestHandler<DeletePriceListItemCommand, Response<string>>
    {
        private readonly IRepositoryAsync<PriceListItem> _itemRepo;
        private readonly IOutputCacheStore _outputCacheStored;

        public DeletePriceListItemCommandHandler(
            IRepositoryAsync<PriceListItem> itemRepo,
            IOutputCacheStore outputCacheStored)
        {
            _itemRepo = itemRepo;
            _outputCacheStored = outputCacheStored;
        }

        public async Task<Response<string>> Handle(DeletePriceListItemCommand request, CancellationToken cancellationToken)
        {
            var item = await _itemRepo.GetByIdAsync(request.Id, cancellationToken);
            if (item == null) throw new KeyNotFoundException($"Item {request.Id} no encontrado");

            await _itemRepo.DeleteAsync(item, cancellationToken);
            await _itemRepo.SaveChangesAsync(cancellationToken);
            await _outputCacheStored.EvictByTagAsync("cache_pricelists", cancellationToken);
            await _outputCacheStored.EvictByTagAsync("cache_products", cancellationToken);
            await _outputCacheStored.EvictByTagAsync("cache_productsEcommerce", cancellationToken);
            return new Response<string>("Precio eliminado", "Eliminado correctamente");
        }
    }
}
