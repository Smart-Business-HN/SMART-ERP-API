using Ardalis.Specification;
using MediatR;
using Microsoft.AspNetCore.OutputCaching;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.PriceListFeature.Commands.DeletePriceListCommand
{
    public class DeletePriceListCommand : IRequest<Response<string>>
    {
        public int Id { get; set; }
    }

    public class DeletePriceListCommandHandler : IRequestHandler<DeletePriceListCommand, Response<string>>
    {
        private readonly IRepositoryAsync<PriceList> _repositoryAsync;
        private readonly IReadRepositoryAsync<Customer> _customerRepo;
        private readonly IReadRepositoryAsync<CustomerType> _customerTypeRepo;
        private readonly IOutputCacheStore _outputCacheStored;

        public DeletePriceListCommandHandler(
            IRepositoryAsync<PriceList> repositoryAsync,
            IReadRepositoryAsync<Customer> customerRepo,
            IReadRepositoryAsync<CustomerType> customerTypeRepo,
            IOutputCacheStore outputCacheStored)
        {
            _repositoryAsync = repositoryAsync;
            _customerRepo = customerRepo;
            _customerTypeRepo = customerTypeRepo;
            _outputCacheStored = outputCacheStored;
        }

        public async Task<Response<string>> Handle(DeletePriceListCommand request, CancellationToken cancellationToken)
        {
            var pl = await _repositoryAsync.GetByIdAsync(request.Id, cancellationToken);
            if (pl == null)
            {
                throw new KeyNotFoundException($"No se encontro lista de precios con el id {request.Id}");
            }

            if (pl.IsDefault)
            {
                throw new ApiException("No puedes eliminar la lista de precios marcada como default.");
            }

            var customerCount = await _customerRepo.CountAsync(new CustomersByPriceListSpec(request.Id), cancellationToken);
            if (customerCount > 0)
            {
                throw new ApiException($"No puedes eliminar la lista: {customerCount} clientes la tienen asignada.");
            }

            var typeCount = await _customerTypeRepo.CountAsync(new CustomerTypesByPriceListSpec(request.Id), cancellationToken);
            if (typeCount > 0)
            {
                throw new ApiException($"No puedes eliminar la lista: {typeCount} tipos de cliente la tienen asignada.");
            }

            await _repositoryAsync.DeleteAsync(pl, cancellationToken);
            await _repositoryAsync.SaveChangesAsync(cancellationToken);
            await _outputCacheStored.EvictByTagAsync("cache_pricelists", cancellationToken);
            await _outputCacheStored.EvictByTagAsync("cache_products", cancellationToken);
            await _outputCacheStored.EvictByTagAsync("cache_productsEcommerce", cancellationToken);
            return new Response<string>($"{pl.Name} eliminado correctamente", "Eliminado correctamente");
        }

        private sealed class CustomersByPriceListSpec : Specification<Customer>
        {
            public CustomersByPriceListSpec(int priceListId)
            {
                Query.Where(x => x.PriceListId == priceListId);
            }
        }

        private sealed class CustomerTypesByPriceListSpec : Specification<CustomerType>
        {
            public CustomerTypesByPriceListSpec(int priceListId)
            {
                Query.Where(x => x.PriceListId == priceListId);
            }
        }
    }
}
