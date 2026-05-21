using MediatR;
using Microsoft.AspNetCore.OutputCaching;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.PriceListFeature.Commands.AssignPriceListToCustomerTypeCommand
{
    public class AssignPriceListToCustomerTypeCommand : IRequest<Response<string>>
    {
        public int CustomerTypeId { get; set; }
        public int? PriceListId { get; set; }
    }

    public class AssignPriceListToCustomerTypeCommandHandler : IRequestHandler<AssignPriceListToCustomerTypeCommand, Response<string>>
    {
        private readonly IRepositoryAsync<CustomerType> _customerTypeRepo;
        private readonly IReadRepositoryAsync<PriceList> _listRepo;
        private readonly IOutputCacheStore _outputCacheStored;

        public AssignPriceListToCustomerTypeCommandHandler(
            IRepositoryAsync<CustomerType> customerTypeRepo,
            IReadRepositoryAsync<PriceList> listRepo,
            IOutputCacheStore outputCacheStored)
        {
            _customerTypeRepo = customerTypeRepo;
            _listRepo = listRepo;
            _outputCacheStored = outputCacheStored;
        }

        public async Task<Response<string>> Handle(AssignPriceListToCustomerTypeCommand request, CancellationToken cancellationToken)
        {
            var ct = await _customerTypeRepo.GetByIdAsync(request.CustomerTypeId, cancellationToken);
            if (ct == null) throw new KeyNotFoundException($"Tipo de cliente {request.CustomerTypeId} no encontrado");

            if (request.PriceListId.HasValue)
            {
                var list = await _listRepo.GetByIdAsync(request.PriceListId.Value, cancellationToken);
                if (list == null) throw new KeyNotFoundException($"Lista de precios {request.PriceListId} no encontrada");
            }

            ct.PriceListId = request.PriceListId;
            await _customerTypeRepo.UpdateAsync(ct, cancellationToken);
            await _customerTypeRepo.SaveChangesAsync(cancellationToken);

            await _outputCacheStored.EvictByTagAsync("cache_pricelists", cancellationToken);
            await _outputCacheStored.EvictByTagAsync("cache_products", cancellationToken);
            await _outputCacheStored.EvictByTagAsync("cache_productsEcommerce", cancellationToken);
            return new Response<string>("ok", request.PriceListId.HasValue
                ? "Lista de precios asignada al tipo de cliente"
                : "Lista removida del tipo de cliente");
        }
    }
}
