using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Provider;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.PurchaseOrderSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.ProviderFeature.Queries
{
    public class GetProviderPurchaseOrdersQuery : IRequest<PagedResponse<List<ProviderPurchaseOrderLineDto>>>
    {
        public int ProviderId { get; set; }
        public string? Parameter { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }

    public class GetProviderPurchaseOrdersQueryHandler : IRequestHandler<GetProviderPurchaseOrdersQuery, PagedResponse<List<ProviderPurchaseOrderLineDto>>>
    {
        private readonly IRepositoryAsync<PurchaseOrder> _repositoryAsync;
        private readonly IMapper _mapper;

        public GetProviderPurchaseOrdersQueryHandler(IRepositoryAsync<PurchaseOrder> repositoryAsync, IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _mapper = mapper;
        }

        public async Task<PagedResponse<List<ProviderPurchaseOrderLineDto>>> Handle(GetProviderPurchaseOrdersQuery request, CancellationToken cancellationToken)
        {
            var pageNumber = request.PageNumber < 0 ? 0 : request.PageNumber;
            var pageSize = request.PageSize <= 0 ? 10 : request.PageSize;

            var orders = await _repositoryAsync.ListAsync(
                new FilterPurchaseOrderByProviderIdSpecification(request.ProviderId, request.Parameter, pageNumber, pageSize),
                cancellationToken);

            var totalItems = await _repositoryAsync.CountAsync(
                new CountPurchaseOrderByProviderIdSpecification(request.ProviderId, request.Parameter),
                cancellationToken);

            var dto = _mapper.Map<List<ProviderPurchaseOrderLineDto>>(orders);
            return new PagedResponse<List<ProviderPurchaseOrderLineDto>>(dto, pageNumber, pageSize, totalItems);
        }
    }
}
