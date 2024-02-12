using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.PurchaseBill;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.PurchaseBillSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.PurchaseBillFeature.Queries
{
    public class GetPurchaseBillByIdQuery : IRequest<Response<PurchaseBillDto>>
    {
        public int Id { get; set; }
    }
    public class GetPurchaseBillByIdQueryHandler : IRequestHandler<GetPurchaseBillByIdQuery,Response<PurchaseBillDto>>
    {
        private readonly IRepositoryAsync<PurchaseBill> _repositoryAsync;
        private readonly IMapper _mapper;
        public GetPurchaseBillByIdQueryHandler(IRepositoryAsync<PurchaseBill> repositoryAsync, IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _mapper = mapper;
        }
        public async Task<Response<PurchaseBillDto>> Handle(GetPurchaseBillByIdQuery request, CancellationToken cancellationToken)
        {
            var getPurchaseBill = await _repositoryAsync.FirstOrDefaultAsync(new FilterPurchaseBillByIdSpecification(request.Id));
            if (getPurchaseBill == null)
            {
                throw new KeyNotFoundException($"Registro no encontrado con el id {request.Id}");
            }
            var dto = _mapper.Map<PurchaseBillDto>(getPurchaseBill);
            return new Response<PurchaseBillDto>(dto);
        }
    }
}
