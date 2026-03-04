using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.ProviderWarehouse;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.ProviderWarehouseSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.ProviderWarehouseFeature.Queries
{
    public class GetProviderWarehousesByProviderQuery : IRequest<Response<List<ProviderWarehouseDto>>>
    {
        public int ProviderId { get; set; }

        public class GetProviderWarehousesByProviderQueryHandler : IRequestHandler<GetProviderWarehousesByProviderQuery, Response<List<ProviderWarehouseDto>>>
        {
            private readonly IMapper _mapper;
            private readonly IRepositoryAsync<ProviderWarehouse> _repositoryAsync;

            public GetProviderWarehousesByProviderQueryHandler(IMapper mapper, IRepositoryAsync<ProviderWarehouse> repositoryAsync)
            {
                _mapper = mapper;
                _repositoryAsync = repositoryAsync;
            }

            public async Task<Response<List<ProviderWarehouseDto>>> Handle(GetProviderWarehousesByProviderQuery request, CancellationToken cancellationToken)
            {
                var spec = new FilterProviderWarehouseByProviderWithDetailsSpec(request.ProviderId);
                var providerWarehouses = await _repositoryAsync.ListAsync(spec);

                var dto = _mapper.Map<List<ProviderWarehouseDto>>(providerWarehouses);
                return new Response<List<ProviderWarehouseDto>>(dto);
            }
        }
    }
}
