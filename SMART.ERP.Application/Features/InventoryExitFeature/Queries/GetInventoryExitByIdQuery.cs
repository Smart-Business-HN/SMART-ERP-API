using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.InventoryExit;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.InventoryExitSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.InventoryExitFeature.Queries
{
    public class GetInventoryExitByIdQuery : IRequest<Response<InventoryExitDto>>
    {
        public int Id { get; set; }

        public class GetInventoryExitByIdQueryHandler : IRequestHandler<GetInventoryExitByIdQuery, Response<InventoryExitDto>>
        {
            private readonly IMapper _mapper;
            private readonly IReadRepositoryAsync<InventoryExit> _repositoryAsync;

            public GetInventoryExitByIdQueryHandler(IMapper mapper, IReadRepositoryAsync<InventoryExit> repositoryAsync)
            {
                _mapper = mapper;
                _repositoryAsync = repositoryAsync;
            }

            public async Task<Response<InventoryExitDto>> Handle(GetInventoryExitByIdQuery request, CancellationToken cancellationToken)
            {
                var exit = await _repositoryAsync.FirstOrDefaultAsync(new GetInventoryExitByIdSpecification(request.Id), cancellationToken)
                    ?? throw new ApiException($"No existe una salida de inventario con el Id {request.Id}");
                var dto = _mapper.Map<InventoryExitDto>(exit);
                return new Response<InventoryExitDto>(dto);
            }
        }
    }
}
