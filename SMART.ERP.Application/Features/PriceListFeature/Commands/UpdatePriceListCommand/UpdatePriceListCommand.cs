using MediatR;
using Microsoft.AspNetCore.OutputCaching;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.PriceListSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.PriceListFeature.Commands.UpdatePriceListCommand
{
    public class UpdatePriceListCommand : IRequest<Response<PriceList>>
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public bool IsDefault { get; set; }
        public bool IsActive { get; set; }
    }

    public class UpdatePriceListCommandHandler : IRequestHandler<UpdatePriceListCommand, Response<PriceList>>
    {
        private readonly IRepositoryAsync<PriceList> _repositoryAsync;
        private readonly IOutputCacheStore _outputCacheStored;

        public UpdatePriceListCommandHandler(IRepositoryAsync<PriceList> repositoryAsync, IOutputCacheStore outputCacheStored)
        {
            _repositoryAsync = repositoryAsync;
            _outputCacheStored = outputCacheStored;
        }

        public async Task<Response<PriceList>> Handle(UpdatePriceListCommand request, CancellationToken cancellationToken)
        {
            var pl = await _repositoryAsync.GetByIdAsync(request.Id, cancellationToken);
            if (pl == null)
            {
                throw new KeyNotFoundException($"No se encontro lista de precios con el id {request.Id}");
            }

            var dup = await _repositoryAsync.FirstOrDefaultAsync(
                new FilterPriceListSpecification(request.Name, request.Id), cancellationToken);
            if (dup != null)
            {
                throw new ApiException($"Ya existe otra lista de precios con el nombre {request.Name}");
            }

            if (request.IsDefault && !pl.IsDefault)
            {
                var current = await _repositoryAsync.ListAsync(cancellationToken);
                foreach (var other in current.Where(x => x.IsDefault && x.Id != request.Id))
                {
                    other.IsDefault = false;
                    await _repositoryAsync.UpdateAsync(other, cancellationToken);
                }
            }

            if (pl.IsDefault && !request.IsDefault)
            {
                throw new ApiException("No puedes desmarcar la lista por defecto sin asignar otra como default.");
            }

            pl.Name = request.Name;
            pl.Description = request.Description;
            pl.IsDefault = request.IsDefault;
            pl.IsActive = request.IsActive;
            pl.ModificationDate = DateTime.UtcNow;
            pl.ModificatedBy = "admin";

            await _repositoryAsync.UpdateAsync(pl, cancellationToken);
            await _repositoryAsync.SaveChangesAsync(cancellationToken);
            await _outputCacheStored.EvictByTagAsync("cache_pricelists", cancellationToken);
            await _outputCacheStored.EvictByTagAsync("cache_products", cancellationToken);
            await _outputCacheStored.EvictByTagAsync("cache_productsEcommerce", cancellationToken);
            return new Response<PriceList>(pl, message: $"{pl.Name} actualizado correctamente");
        }
    }
}
