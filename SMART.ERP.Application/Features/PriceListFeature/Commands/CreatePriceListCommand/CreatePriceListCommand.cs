using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.OutputCaching;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.PriceListSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.PriceListFeature.Commands.CreatePriceListCommand
{
    public class CreatePriceListCommand : IRequest<Response<PriceList>>
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public bool IsDefault { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class CreatePriceListCommandHandler : IRequestHandler<CreatePriceListCommand, Response<PriceList>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<PriceList> _repositoryAsync;
        private readonly IOutputCacheStore _outputCacheStored;

        public CreatePriceListCommandHandler(IMapper mapper, IRepositoryAsync<PriceList> repositoryAsync, IOutputCacheStore outputCacheStored)
        {
            _mapper = mapper;
            _repositoryAsync = repositoryAsync;
            _outputCacheStored = outputCacheStored;
        }

        public async Task<Response<PriceList>> Handle(CreatePriceListCommand request, CancellationToken cancellationToken)
        {
            var existing = await _repositoryAsync.FirstOrDefaultAsync(
                new FilterPriceListSpecification(request.Name, null), cancellationToken);
            if (existing != null)
            {
                throw new ApiException($"Ya existe una lista de precios con el nombre {request.Name}");
            }

            if (request.IsDefault)
            {
                var current = await _repositoryAsync.ListAsync(cancellationToken);
                foreach (var pl in current.Where(x => x.IsDefault))
                {
                    pl.IsDefault = false;
                    await _repositoryAsync.UpdateAsync(pl, cancellationToken);
                }
            }

            var newRecord = _mapper.Map<PriceList>(request);
            newRecord.CreationDate = DateTime.UtcNow;
            newRecord.CreatedBy = "admin";

            var data = await _repositoryAsync.AddAsync(newRecord, cancellationToken);
            await _repositoryAsync.SaveChangesAsync(cancellationToken);
            await _outputCacheStored.EvictByTagAsync("cache_pricelists", cancellationToken);
            await _outputCacheStored.EvictByTagAsync("cache_products", cancellationToken);
            await _outputCacheStored.EvictByTagAsync("cache_productsEcommerce", cancellationToken);
            return new Response<PriceList>(data, message: $"{request.Name} creado exitosamente");
        }
    }
}
