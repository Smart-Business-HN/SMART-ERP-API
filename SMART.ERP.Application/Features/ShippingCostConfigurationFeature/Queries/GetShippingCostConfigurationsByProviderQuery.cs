using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.ShippingCost;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.ShippingCostConfigurationSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.ShippingCostConfigurationFeature.Queries
{
    public class GetShippingCostConfigurationsByProviderQuery : IRequest<Response<List<ShippingCostDto>>>
    {
        public int ProviderId { get; set; }

        public class GetShippingCostConfigurationsByProviderQueryHandler : IRequestHandler<GetShippingCostConfigurationsByProviderQuery, Response<List<ShippingCostDto>>>
        {
            private readonly IMapper _mapper;
            private readonly IRepositoryAsync<ShippingCostConfiguration> _repositoryAsync;

            public GetShippingCostConfigurationsByProviderQueryHandler(IMapper mapper, IRepositoryAsync<ShippingCostConfiguration> repositoryAsync)
            {
                _mapper = mapper;
                _repositoryAsync = repositoryAsync;
            }

            public async Task<Response<List<ShippingCostDto>>> Handle(GetShippingCostConfigurationsByProviderQuery request, CancellationToken cancellationToken)
            {
                var spec = new FilterShippingCostConfigurationsByProviderSpec(request.ProviderId);
                var configurations = await _repositoryAsync.ListAsync(spec);

                var dto = _mapper.Map<List<ShippingCostDto>>(configurations);
                return new Response<List<ShippingCostDto>>(dto);
            }
        }
    }
}
