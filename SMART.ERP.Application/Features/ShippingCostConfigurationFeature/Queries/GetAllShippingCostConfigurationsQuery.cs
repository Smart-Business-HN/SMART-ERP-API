using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.ShippingCost;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.ShippingCostConfigurationSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.ShippingCostConfigurationFeature.Queries
{
    public class GetAllShippingCostConfigurationsQuery : IRequest<PagedResponse<List<ShippingCostDto>>>
    {
        public string? Parameter { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? Order { get; set; }
        public string? Column { get; set; }
        public bool All { get; set; }

        public class GetAllShippingCostConfigurationsQueryHandler : IRequestHandler<GetAllShippingCostConfigurationsQuery, PagedResponse<List<ShippingCostDto>>>
        {
            private readonly IMapper _mapper;
            private readonly IRepositoryAsync<ShippingCostConfiguration> _repositoryAsync;

            public GetAllShippingCostConfigurationsQueryHandler(IMapper mapper, IRepositoryAsync<ShippingCostConfiguration> repositoryAsync)
            {
                _mapper = mapper;
                _repositoryAsync = repositoryAsync;
            }

            public async Task<PagedResponse<List<ShippingCostDto>>> Handle(GetAllShippingCostConfigurationsQuery request, CancellationToken cancellationToken)
            {
                if (request.All)
                {
                    request.PageNumber = 0;
                    request.PageSize = await _repositoryAsync.CountAsync();
                }

                var configurations = await _repositoryAsync.ListAsync(
                    new FilterAndPaginationShippingCostConfigurationSpec(
                        request.Parameter,
                        request.PageNumber,
                        request.PageSize,
                        request.Order,
                        request.Column));

                var dto = _mapper.Map<List<ShippingCostDto>>(configurations);
                return new PagedResponse<List<ShippingCostDto>>(
                    dto,
                    request.PageNumber,
                    request.PageSize,
                    request.All ? request.PageSize : await _repositoryAsync.CountAsync());
            }
        }
    }
}
