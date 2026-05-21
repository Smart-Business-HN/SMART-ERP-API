using MediatR;
using Microsoft.AspNetCore.OutputCaching;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.PriceListSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.PriceListFeature.Commands.ClonePriceListCommand
{
    public class ClonePriceListCommand : IRequest<Response<PriceList>>
    {
        public int SourcePriceListId { get; set; }
        public string NewName { get; set; } = null!;
        public string? NewDescription { get; set; }
    }

    public class ClonePriceListCommandHandler : IRequestHandler<ClonePriceListCommand, Response<PriceList>>
    {
        private readonly IRepositoryAsync<PriceList> _listRepo;
        private readonly IRepositoryAsync<PriceListItem> _itemRepo;
        private readonly IOutputCacheStore _outputCacheStored;

        public ClonePriceListCommandHandler(
            IRepositoryAsync<PriceList> listRepo,
            IRepositoryAsync<PriceListItem> itemRepo,
            IOutputCacheStore outputCacheStored)
        {
            _listRepo = listRepo;
            _itemRepo = itemRepo;
            _outputCacheStored = outputCacheStored;
        }

        public async Task<Response<PriceList>> Handle(ClonePriceListCommand request, CancellationToken cancellationToken)
        {
            var source = await _listRepo.GetByIdAsync(request.SourcePriceListId, cancellationToken);
            if (source == null) throw new KeyNotFoundException($"Lista origen {request.SourcePriceListId} no encontrada");

            var dup = await _listRepo.FirstOrDefaultAsync(new FilterPriceListSpecification(request.NewName, null), cancellationToken);
            if (dup != null) throw new ApiException($"Ya existe una lista con el nombre {request.NewName}");

            var newList = new PriceList
            {
                Name = request.NewName,
                Description = request.NewDescription ?? source.Description,
                IsDefault = false,
                IsActive = true,
                CreationDate = DateTime.UtcNow,
                CreatedBy = "admin"
            };
            await _listRepo.AddAsync(newList, cancellationToken);
            await _listRepo.SaveChangesAsync(cancellationToken);

            var sourceItems = await _itemRepo.ListAsync(new PriceListItemsByListSpecification(request.SourcePriceListId, null, 0, int.MaxValue), cancellationToken);
            foreach (var src in sourceItems)
            {
                await _itemRepo.AddAsync(new PriceListItem
                {
                    PriceListId = newList.Id,
                    ProductId = src.ProductId,
                    Price = src.Price,
                    CreationDate = DateTime.UtcNow,
                    CreatedBy = "admin"
                }, cancellationToken);
            }
            await _itemRepo.SaveChangesAsync(cancellationToken);

            await _outputCacheStored.EvictByTagAsync("cache_pricelists", cancellationToken);
            return new Response<PriceList>(newList, $"Lista clonada con {sourceItems.Count} items");
        }
    }
}
