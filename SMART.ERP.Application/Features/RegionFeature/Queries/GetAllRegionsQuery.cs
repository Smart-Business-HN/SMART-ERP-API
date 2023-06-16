using AutoMapper;
using MediatR;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.RegionSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Application.DTOs.Address;

namespace SMART.ERP.Application.Features.RegionFeature.Queries
{
    public class GetAllRegionsQuery : IRequest<PagedResponse<List<RegionDto>>>
    {
        public string? Parameter { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? Order { get; set; }
        public string? Column { get; set; }
        public bool All { get; set; }
    }

    public class GetAllRegionsQueryHandler : IRequestHandler<GetAllRegionsQuery, PagedResponse<List<RegionDto>>>
    {
        private readonly IRepositoryAsync<Region> _repositoryAsync;
        private readonly IMapper _mapper;

        public GetAllRegionsQueryHandler(IRepositoryAsync<Region> repositoryAsync, IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _mapper = mapper;
        }

        public async Task<PagedResponse<List<RegionDto>>> Handle(GetAllRegionsQuery request, CancellationToken cancellationToken)
        {
            if (request.All)
            {
                request.PageNumber = 0;
                request.Parameter = null;
                request.Order = null;
                request.Column = null;
                request.PageSize = await _repositoryAsync.CountAsync();
            }
            var regions = await _repositoryAsync.ListAsync(new FilterAndPaginationRegionSpecification(request.Parameter, request.Order, request.Column));
            var dto = _mapper.Map<List<RegionDto>>(regions);
            dto = dto.Skip(request.PageNumber * request.PageSize).Take(request.PageSize).ToList();
            return new PagedResponse<List<RegionDto>>(dto, request.PageNumber, request.PageSize, request.All ? request.PageSize : regions.Count);
        }
    }
}
