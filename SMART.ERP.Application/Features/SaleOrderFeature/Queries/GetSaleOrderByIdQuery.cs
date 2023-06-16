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
    public class GetSaleOrderByIdQuery : IRequest<Response<SaleOrderDto>>
    {
        public int Id { get; set; }
    }

    public class GetSaleOrderByIdQueryHandler : IRequestHandler<GetSaleOrderByIdQuery, Response<SaleOrderDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<SaleOrder> _repositoryAsync;
        private readonly IRepositoryHNAsync<Client> _repositoryHNAsync;
        public GetSaleOrderByIdQueryHandler(IMapper mapper, IRepositoryAsync<SaleOrder> repositoryAsync,
            IRepositoryHNAsync<Client> repositoryHNAsync)
        {
            _mapper = mapper;
            _repositoryAsync = repositoryAsync;
            _repositoryHNAsync = repositoryHNAsync;
        }
        public async Task<Response<SaleOrderDto>> Handle(GetSaleOrderByIdQuery request, CancellationToken cancellationToken)
        {
            var saleOrder = await _repositoryAsync.FirstOrDefaultAsync(
                new SaleOrderIncludesSpecification(id: request.Id));
            if (saleOrder == null)
            {
                throw new KeyNotFoundException($"Registro no encontrado con el id {request.Id}");
            }
            var dto = _mapper.Map<SaleOrderDto>(saleOrder);
            var customer = await _repositoryHNAsync.GetByIdAsync(dto.Customer!.MasterId);
            if (customer != null)
            {
                dto.Customer!.FullName = customer.FullName;
                dto.Customer!.PhoneNumber = customer.PhoneNumber;
                dto.Customer!.Email = customer.Email;
            }
            return new Response<SaleOrderDto>(dto);
        }
    }
}
