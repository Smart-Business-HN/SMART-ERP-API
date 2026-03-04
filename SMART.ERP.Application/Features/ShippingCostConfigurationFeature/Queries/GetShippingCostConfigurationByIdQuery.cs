using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.ShippingCost;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.ShippingCostConfigurationSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.ShippingCostConfigurationFeature.Queries
{
    public class GetShippingCostConfigurationByIdQuery : IRequest<Response<ShippingCostDto>>
    {
        public int Id { get; set; }

        public class GetShippingCostConfigurationByIdQueryHandler : IRequestHandler<GetShippingCostConfigurationByIdQuery, Response<ShippingCostDto>>
        {
            private readonly IMapper _mapper;
            private readonly IRepositoryAsync<ShippingCostConfiguration> _repositoryAsync;

            public GetShippingCostConfigurationByIdQueryHandler(IMapper mapper, IRepositoryAsync<ShippingCostConfiguration> repositoryAsync)
            {
                _mapper = mapper;
                _repositoryAsync = repositoryAsync;
            }

            public async Task<Response<ShippingCostDto>> Handle(GetShippingCostConfigurationByIdQuery request, CancellationToken cancellationToken)
            {
                var spec = new FilterShippingCostConfigurationByIdWithDetailsSpec(request.Id);
                var configuration = await _repositoryAsync.FirstOrDefaultAsync(spec);

                if (configuration == null)
                {
                    throw new ApiException($"Configuración con ID {request.Id} no encontrada");
                }

                var dto = _mapper.Map<ShippingCostDto>(configuration);
                return new Response<ShippingCostDto>(dto);
            }
        }
    }
}
