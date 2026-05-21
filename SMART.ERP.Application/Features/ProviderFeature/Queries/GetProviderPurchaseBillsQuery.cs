using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Provider;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.PurchaseBillSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.ProviderFeature.Queries
{
    public class GetProviderPurchaseBillsQuery : IRequest<PagedResponse<List<ProviderPurchaseBillLineDto>>>
    {
        public int ProviderId { get; set; }
        public string? Parameter { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }

    public class GetProviderPurchaseBillsQueryHandler : IRequestHandler<GetProviderPurchaseBillsQuery, PagedResponse<List<ProviderPurchaseBillLineDto>>>
    {
        private readonly IRepositoryAsync<PurchaseBill> _repositoryAsync;
        private readonly IMapper _mapper;

        public GetProviderPurchaseBillsQueryHandler(IRepositoryAsync<PurchaseBill> repositoryAsync, IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _mapper = mapper;
        }

        public async Task<PagedResponse<List<ProviderPurchaseBillLineDto>>> Handle(GetProviderPurchaseBillsQuery request, CancellationToken cancellationToken)
        {
            var pageNumber = request.PageNumber < 0 ? 0 : request.PageNumber;
            var pageSize = request.PageSize <= 0 ? 10 : request.PageSize;

            var bills = await _repositoryAsync.ListAsync(
                new FilterPurchaseBillByProviderIdSpecification(request.ProviderId, request.Parameter, pageNumber, pageSize),
                cancellationToken);

            var totalItems = await _repositoryAsync.CountAsync(
                new CountPurchaseBillByProviderIdSpecification(request.ProviderId, request.Parameter),
                cancellationToken);

            var dto = _mapper.Map<List<ProviderPurchaseBillLineDto>>(bills);
            return new PagedResponse<List<ProviderPurchaseBillLineDto>>(dto, pageNumber, pageSize, totalItems);
        }
    }
}
