using Ardalis.Specification;
using MediatR;
using SMART.ERP.Application.DTOs.PriceList;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.PriceListResolver;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.PriceListFeature.Queries
{
    public class GetEffectivePriceListByCustomerIdQuery : IRequest<Response<EffectivePriceListDto>>
    {
        public Guid CustomerId { get; set; }

        public class GetEffectivePriceListByCustomerIdQueryHandler : IRequestHandler<GetEffectivePriceListByCustomerIdQuery, Response<EffectivePriceListDto>>
        {
            private readonly IReadRepositoryAsync<Customer> _customerRepo;
            private readonly IReadRepositoryAsync<PriceList> _priceListRepo;
            private readonly IPriceListService _priceListService;

            public GetEffectivePriceListByCustomerIdQueryHandler(
                IReadRepositoryAsync<Customer> customerRepo,
                IReadRepositoryAsync<PriceList> priceListRepo,
                IPriceListService priceListService)
            {
                _customerRepo = customerRepo;
                _priceListRepo = priceListRepo;
                _priceListService = priceListService;
            }

            public async Task<Response<EffectivePriceListDto>> Handle(GetEffectivePriceListByCustomerIdQuery request, CancellationToken cancellationToken)
            {
                var customer = await _customerRepo.FirstOrDefaultAsync(new CustomerWithTypeSpec(request.CustomerId), cancellationToken);
                if (customer == null) throw new KeyNotFoundException($"Cliente {request.CustomerId} no encontrado");

                string source;
                if (customer.PriceListId.HasValue)
                    source = "customer";
                else if (customer.CustomerType?.PriceListId.HasValue == true)
                    source = "customerType";
                else
                    source = "default";

                var resolvedId = await _priceListService.ResolvePriceListIdAsync(
                    customerId: request.CustomerId, ct: cancellationToken);

                if (resolvedId == null)
                {
                    return new Response<EffectivePriceListDto>(new EffectivePriceListDto { Source = "none" });
                }

                var list = await _priceListRepo.GetByIdAsync(resolvedId.Value, cancellationToken);
                return new Response<EffectivePriceListDto>(new EffectivePriceListDto
                {
                    PriceListId = resolvedId,
                    PriceListName = list?.Name,
                    Source = source
                });
            }

            private sealed class CustomerWithTypeSpec : Specification<Customer>
            {
                public CustomerWithTypeSpec(Guid id)
                {
                    Query.Where(x => x.Id == id).Include(x => x.CustomerType).AsNoTracking();
                }
            }
        }
    }
}
