using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.PriceList;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.PriceListSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.PriceListFeature.Queries
{
    public class GetPriceListByIdQuery : IRequest<Response<PriceListDto>>
    {
        public int Id { get; set; }

        public class GetPriceListByIdQueryHandler : IRequestHandler<GetPriceListByIdQuery, Response<PriceListDto>>
        {
            private readonly IMapper _mapper;
            private readonly IRepositoryAsync<PriceList> _repositoryAsync;
            private readonly IReadRepositoryAsync<PriceListItem> _itemRepo;

            public GetPriceListByIdQueryHandler(IMapper mapper, IRepositoryAsync<PriceList> repositoryAsync, IReadRepositoryAsync<PriceListItem> itemRepo)
            {
                _mapper = mapper;
                _repositoryAsync = repositoryAsync;
                _itemRepo = itemRepo;
            }

            public async Task<Response<PriceListDto>> Handle(GetPriceListByIdQuery request, CancellationToken cancellationToken)
            {
                var pl = await _repositoryAsync.GetByIdAsync(request.Id, cancellationToken);
                if (pl == null) throw new KeyNotFoundException($"Registro no encontrado con el id {request.Id}");

                var dto = _mapper.Map<PriceListDto>(pl);
                dto.ItemsCount = await _itemRepo.CountAsync(new PriceListItemsByListSpecification(dto.Id, null, 0, int.MaxValue), cancellationToken);
                return new Response<PriceListDto>(dto);
            }
        }
    }
}
