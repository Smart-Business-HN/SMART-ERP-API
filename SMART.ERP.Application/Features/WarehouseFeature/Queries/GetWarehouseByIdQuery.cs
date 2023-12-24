using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Warehouse;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.WarehouseSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.WarehouseFeature.Queries
{
    public class GetWarehouseByIdQuery : IRequest<Response<WarehouseDto>>
    {
        public int Id { get; set; }
    }

    public class GetWarehouseByIdQueryHandler : IRequestHandler<GetWarehouseByIdQuery, Response<WarehouseDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<Warehouse> _repositoryAsync;

        public GetWarehouseByIdQueryHandler(IMapper mapper, IRepositoryAsync<Warehouse> repositoryAsync)
        {
            _mapper = mapper;
            _repositoryAsync = repositoryAsync;
        }

        public async Task<Response<WarehouseDto>> Handle(GetWarehouseByIdQuery request, CancellationToken cancellationToken)
        {
            var warehouse = await _repositoryAsync.FirstOrDefaultAsync(
                new WarehoseIncludesSpecification(id: request.Id));
            if (warehouse == null)
            {
                throw new KeyNotFoundException($"Registro no encontrado con el id {request.Id}");
            }
            var dto = _mapper.Map<WarehouseDto>(warehouse);
            return new Response<WarehouseDto>(dto);
        }
    }
}
