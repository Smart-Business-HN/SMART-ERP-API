using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.PurchaseBill;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.PurchaseBillSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.PurchaseBillFeature.Queries
{
    public class GetAllPurchaseBillQuery : IRequest<PagedResponse<List<PurchaseBillDto>>>
    {
        public string? Parameter { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? Order { get; set; }
        public string? Column { get; set; }
        public bool All { get; set; }
    }
    public class GetAllPurchaseBillQueryHandler : IRequestHandler<GetAllPurchaseBillQuery, PagedResponse<List<PurchaseBillDto>>>
    {
        private readonly IRepositoryAsync<PurchaseBill> _repositoryAsync;
        private readonly IMapper _mapper;
        public GetAllPurchaseBillQueryHandler(IRepositoryAsync<PurchaseBill> repositoryAsync, IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _mapper = mapper;
        }
        public async Task<PagedResponse<List<PurchaseBillDto>>> Handle(GetAllPurchaseBillQuery request, CancellationToken cancellationToken)
        {
            if (request.All)
            {
                request.PageNumber = 0;
                request.PageSize = await _repositoryAsync.CountAsync();
            }
            var purchaseBills = await _repositoryAsync.ListAsync(new FilterAndPaginationPurchaseBillSpecification(request.Parameter, request.PageNumber, request.PageSize, request.Order, request.Column));
            var dto = _mapper.Map<List<PurchaseBillDto>>(purchaseBills);
            return new PagedResponse<List<PurchaseBillDto>>(dto, request.PageNumber, request.PageSize, request.All ? request.PageSize : await _repositoryAsync.CountAsync());
        }
    }
}
