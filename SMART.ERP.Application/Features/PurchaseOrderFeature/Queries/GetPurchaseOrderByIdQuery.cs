using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.PurchaseOrder;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.PurchaseOrderSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.PurchaseOrderFeature.Queries
{
    public class GetPurchaseOrderByIdQuery : IRequest<Response<PurchaseOrderDto>>
    {
        public int Id { get; set; }
    }
    public class GetPurchaseOrderByIdQueryHandler : IRequestHandler<GetPurchaseOrderByIdQuery, Response<PurchaseOrderDto>>
    {
        private readonly IRepositoryAsync<PurchaseOrder> _repositoryAsync;
        private readonly IMapper _mapper;
        public GetPurchaseOrderByIdQueryHandler(IRepositoryAsync<PurchaseOrder> repositoryAsync, IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _mapper = mapper;
        }
        public async Task<Response<PurchaseOrderDto>> Handle(GetPurchaseOrderByIdQuery request, CancellationToken cancellationToken)
        {
            var getPurchaseOrder = await _repositoryAsync.FirstOrDefaultAsync(new FilterPurchaseOrderByIdSpecification(request.Id));
            if (getPurchaseOrder == null)
            {
                throw new KeyNotFoundException($"Registro no encontrado con el id {request.Id}");
            }
            var dto = _mapper.Map<PurchaseOrderDto>(getPurchaseOrder);
            return new Response<PurchaseOrderDto>(dto);
        }
    }
}
