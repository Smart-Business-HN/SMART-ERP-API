using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.InventoryInput;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.InventoryInputSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.InventoryInputFeature.Queries
{
    public class GetInventoryInputByIdQuery : IRequest<Response<InventoryInputDto>>
    {
        public int Id { get; set; }
    }

    public class GetInventoryInputByIdQueryHandler : IRequestHandler<GetInventoryInputByIdQuery, Response<InventoryInputDto>>
    {
        private readonly IRepositoryAsync<InventoryInput> _repositoryAsync;
        private readonly IMapper _mapper;

        public GetInventoryInputByIdQueryHandler(IRepositoryAsync<InventoryInput> repositoryAsync, IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _mapper = mapper;
        }
        public async Task<Response<InventoryInputDto>> Handle(GetInventoryInputByIdQuery request, CancellationToken cancellationToken)
        {
            var inventoryInputType = await _repositoryAsync.FirstOrDefaultAsync(new InventoryInputIncludesSpecification(request.Id));
            if (inventoryInputType == null)
            {
                throw new KeyNotFoundException($"Registro no encontrado con el id {request.Id}");
            }
            var dto = _mapper.Map<InventoryInputDto>(inventoryInputType);
            return new Response<InventoryInputDto>(dto);
        }
    }
}
