using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.PriceList;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.PriceListSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.PriceListFeature.Queries
{
    public class GetAllPriceListsQuery : IRequest<PagedResponse<List<PriceListDto>>>
    {
        public string? Parameter { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? Order { get; set; }
        public string? Column { get; set; }
        public bool All { get; set; }
    }

    public class GetAllPriceListsQueryHandler : IRequestHandler<GetAllPriceListsQuery, PagedResponse<List<PriceListDto>>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<PriceList> _repositoryAsync;
        private readonly IReadRepositoryAsync<PriceListItem> _itemRepo;

        public GetAllPriceListsQueryHandler(IMapper mapper, IRepositoryAsync<PriceList> repositoryAsync, IReadRepositoryAsync<PriceListItem> itemRepo)
        {
            _mapper = mapper;
            _repositoryAsync = repositoryAsync;
            _itemRepo = itemRepo;
        }

        public async Task<PagedResponse<List<PriceListDto>>> Handle(GetAllPriceListsQuery request, CancellationToken cancellationToken)
        {
            if (request.All)
            {
                request.PageNumber = 0;
                request.PageSize = await _repositoryAsync.CountAsync(cancellationToken);
            }
            var lists = await _repositoryAsync.ListAsync(
                new FilterAndPaginationPriceListSpecification(request.Parameter, request.PageNumber, request.PageSize, request.Order, request.Column),
                cancellationToken);

            var dtos = _mapper.Map<List<PriceListDto>>(lists);
            foreach (var dto in dtos)
            {
                dto.ItemsCount = await _itemRepo.CountAsync(new PriceListItemsByListSpecification(dto.Id, null, 0, int.MaxValue), cancellationToken);
            }

            return new PagedResponse<List<PriceListDto>>(dtos, request.PageNumber, request.PageSize,
                request.All ? request.PageSize : await _repositoryAsync.CountAsync(cancellationToken));
        }
    }
}
