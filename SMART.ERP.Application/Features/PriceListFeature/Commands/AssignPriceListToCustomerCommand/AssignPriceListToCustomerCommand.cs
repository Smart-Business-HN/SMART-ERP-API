using MediatR;
using Microsoft.AspNetCore.OutputCaching;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.PriceListFeature.Commands.AssignPriceListToCustomerCommand
{
    public class AssignPriceListToCustomerCommand : IRequest<Response<string>>
    {
        public Guid CustomerId { get; set; }
        public int? PriceListId { get; set; }
    }

    public class AssignPriceListToCustomerCommandHandler : IRequestHandler<AssignPriceListToCustomerCommand, Response<string>>
    {
        private readonly IRepositoryAsync<Customer> _customerRepo;
        private readonly IReadRepositoryAsync<PriceList> _listRepo;
        private readonly IOutputCacheStore _outputCacheStored;

        public AssignPriceListToCustomerCommandHandler(
            IRepositoryAsync<Customer> customerRepo,
            IReadRepositoryAsync<PriceList> listRepo,
            IOutputCacheStore outputCacheStored)
        {
            _customerRepo = customerRepo;
            _listRepo = listRepo;
            _outputCacheStored = outputCacheStored;
        }

        public async Task<Response<string>> Handle(AssignPriceListToCustomerCommand request, CancellationToken cancellationToken)
        {
            var customer = await _customerRepo.GetByIdAsync(request.CustomerId, cancellationToken);
            if (customer == null) throw new KeyNotFoundException($"Cliente {request.CustomerId} no encontrado");

            if (request.PriceListId.HasValue)
            {
                var list = await _listRepo.GetByIdAsync(request.PriceListId.Value, cancellationToken);
                if (list == null) throw new KeyNotFoundException($"Lista de precios {request.PriceListId} no encontrada");
            }

            customer.PriceListId = request.PriceListId;
            await _customerRepo.UpdateAsync(customer, cancellationToken);
            await _customerRepo.SaveChangesAsync(cancellationToken);

            await _outputCacheStored.EvictByTagAsync("cache_pricelists", cancellationToken);
            await _outputCacheStored.EvictByTagAsync("cache_customer", cancellationToken);
            await _outputCacheStored.EvictByTagAsync("cache_products", cancellationToken);
            await _outputCacheStored.EvictByTagAsync("cache_productsEcommerce", cancellationToken);
            return new Response<string>("ok", request.PriceListId.HasValue
                ? "Lista de precios asignada al cliente"
                : "Override de lista removido (heredará del tipo de cliente)");
        }
    }
}
