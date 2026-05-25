using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.InventoryEntry;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.InventoryEntrySpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.InventoryEntryFeature.Queries
{
    public class GetInventoryEntryByIdQuery : IRequest<Response<InventoryEntryDto>>
    {
        public int Id { get; set; }

        public class GetInventoryEntryByIdQueryHandler : IRequestHandler<GetInventoryEntryByIdQuery, Response<InventoryEntryDto>>
        {
            private readonly IMapper _mapper;
            private readonly IReadRepositoryAsync<InventoryEntry> _repositoryAsync;

            public GetInventoryEntryByIdQueryHandler(IMapper mapper, IReadRepositoryAsync<InventoryEntry> repositoryAsync)
            {
                _mapper = mapper;
                _repositoryAsync = repositoryAsync;
            }

            public async Task<Response<InventoryEntryDto>> Handle(GetInventoryEntryByIdQuery request, CancellationToken cancellationToken)
            {
                var entry = await _repositoryAsync.FirstOrDefaultAsync(new GetInventoryEntryByIdSpecification(request.Id), cancellationToken)
                    ?? throw new ApiException($"No existe una entrada de inventario con el Id {request.Id}");
                var dto = _mapper.Map<InventoryEntryDto>(entry);
                return new Response<InventoryEntryDto>(dto);
            }
        }
    }
}
