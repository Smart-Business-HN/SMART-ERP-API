using MediatR;
using Microsoft.AspNetCore.OutputCaching;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.PriceListFeature.Commands.SetDefaultPriceListCommand
{
    public class SetDefaultPriceListCommand : IRequest<Response<string>>
    {
        public int Id { get; set; }
    }

    public class SetDefaultPriceListCommandHandler : IRequestHandler<SetDefaultPriceListCommand, Response<string>>
    {
        private readonly IRepositoryAsync<PriceList> _repositoryAsync;
        private readonly IOutputCacheStore _outputCacheStored;

        public SetDefaultPriceListCommandHandler(IRepositoryAsync<PriceList> repositoryAsync, IOutputCacheStore outputCacheStored)
        {
            _repositoryAsync = repositoryAsync;
            _outputCacheStored = outputCacheStored;
        }

        public async Task<Response<string>> Handle(SetDefaultPriceListCommand request, CancellationToken cancellationToken)
        {
            var target = await _repositoryAsync.GetByIdAsync(request.Id, cancellationToken);
            if (target == null) throw new KeyNotFoundException($"Lista de precios {request.Id} no encontrada");

            var all = await _repositoryAsync.ListAsync(cancellationToken);
            foreach (var pl in all.Where(x => x.IsDefault && x.Id != request.Id))
            {
                pl.IsDefault = false;
                await _repositoryAsync.UpdateAsync(pl, cancellationToken);
            }

            if (!target.IsDefault)
            {
                target.IsDefault = true;
                target.ModificationDate = DateTime.UtcNow;
                target.ModificatedBy = "admin";
                await _repositoryAsync.UpdateAsync(target, cancellationToken);
            }
            await _repositoryAsync.SaveChangesAsync(cancellationToken);

            await _outputCacheStored.EvictByTagAsync("cache_pricelists", cancellationToken);
            await _outputCacheStored.EvictByTagAsync("cache_products", cancellationToken);
            await _outputCacheStored.EvictByTagAsync("cache_productsEcommerce", cancellationToken);
            return new Response<string>($"{target.Name} marcada como default", "Lista por defecto actualizada");
        }
    }
}
