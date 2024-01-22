using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.PurchaseOrder;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.PurchaseOrderSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
namespace SMART.ERP.Application.Features.PurchaseOrderFeature.Queries
{
    public class GetAllPurchaseOrderQuery : IRequest<PagedResponse<List<PurchaseOrderDto>>>
    {
        public string? Parameter { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? Order { get; set; }
        public string? Column { get; set; }
        public bool All { get; set; }
    }
    public class GetAllPurchaseOrderQueryHandler : IRequestHandler<GetAllPurchaseOrderQuery, PagedResponse<List<PurchaseOrderDto>>>
    {
        private readonly IRepositoryAsync<PurchaseOrder> _repositoryAsync;
        private readonly IMapper _mapper;
        public GetAllPurchaseOrderQueryHandler(IRepositoryAsync<PurchaseOrder> repositoryAsync, IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _mapper = mapper;
        }
        public async Task<PagedResponse<List<PurchaseOrderDto>>> Handle(GetAllPurchaseOrderQuery request, CancellationToken cancellationToken)
        {
            if (request.All)
            {
                request.PageNumber = 0;
                request.PageSize = await _repositoryAsync.CountAsync();
            }
            var purchaseOrders = await _repositoryAsync.ListAsync(new FilterAndPaginationPurchaseOrderSpecification(request.Parameter, request.PageNumber, request.PageSize, request.Order, request.Column));
            var dto = _mapper.Map<List<PurchaseOrderDto>>(purchaseOrders);
            return new PagedResponse<List<PurchaseOrderDto>>(dto, request.PageNumber, request.PageSize, request.All ? request.PageSize : await _repositoryAsync.CountAsync());
        }
    }
}
