using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.SaleOrder;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.SaleOrderSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.MASTER.Domain.Entities;

namespace SMART.ERP.Application.Features.SaleOrderFeature.Queries
{
    public class GetAllSaleOrdersQuery : IRequest<PagedResponse<List<SaleOrderDto>>>
    {
        public string? Parameter { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? Order { get; set; }
        public string? Column { get; set; }
        public class GetAllSaleOrdersQueryHandler : IRequestHandler<GetAllSaleOrdersQuery, PagedResponse<List<SaleOrderDto>>>
        {
            private readonly IMapper _mapper;
            private readonly IRepositoryAsync<SaleOrder> _repositoryAsync;
            private readonly IRepositoryHNAsync<Client> _repositoryHNAsync;

            public GetAllSaleOrdersQueryHandler(IMapper mapper, IRepositoryAsync<SaleOrder> repositoryAsync,
                IRepositoryHNAsync<Client> repositoryHNAsync)
            {
                _mapper = mapper;
                _repositoryAsync = repositoryAsync;
                _repositoryHNAsync = repositoryHNAsync;
            }
            public async Task<PagedResponse<List<SaleOrderDto>>> Handle(GetAllSaleOrdersQuery request, CancellationToken cancellationToken)
            {
                var saleOrders = await _repositoryAsync.ListAsync(
                    new FilterAndPaginationSaleOrderSpecification(request.Parameter, request.PageNumber, request.PageSize, request.Order, request.Column));
                var dto = _mapper.Map<List<SaleOrderDto>>(saleOrders);
                for (int index = 0; index < dto.Count; index++)
                {
                    var customer = await _repositoryHNAsync.GetByIdAsync(dto[index]!.Customer!.MasterId);
                    if (customer != null)
                    {
                        dto[index].Customer!.FullName = customer.FullName;
                        dto[index].Customer!.PhoneNumber = customer.PhoneNumber;
                        dto[index].Customer!.Email = customer.Email;
                    }
                }
                return new PagedResponse<List<SaleOrderDto>>(dto, request.PageNumber, request.PageSize, await _repositoryAsync.CountAsync());
            }
        }
    }
}
