using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.ProviderWarehouse;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.ProviderWarehouseSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.ProviderWarehouseFeature.Queries
{
    public class GetAllProviderWarehousesQuery : IRequest<Response<List<ProviderWarehouseDto>>>
    {
        public class GetAllProviderWarehousesQueryHandler : IRequestHandler<GetAllProviderWarehousesQuery, Response<List<ProviderWarehouseDto>>>
        {
            private readonly IMapper _mapper;
            private readonly IRepositoryAsync<ProviderWarehouse> _repositoryAsync;

            public GetAllProviderWarehousesQueryHandler(IMapper mapper, IRepositoryAsync<ProviderWarehouse> repositoryAsync)
            {
                _mapper = mapper;
                _repositoryAsync = repositoryAsync;
            }

            public async Task<Response<List<ProviderWarehouseDto>>> Handle(GetAllProviderWarehousesQuery request, CancellationToken cancellationToken)
            {
                var spec = new FilterProviderWarehouseWithDetailsSpec();
                var providerWarehouses = await _repositoryAsync.ListAsync(spec);

                var dto = _mapper.Map<List<ProviderWarehouseDto>>(providerWarehouses);
                return new Response<List<ProviderWarehouseDto>>(dto);
            }
        }
    }
}
