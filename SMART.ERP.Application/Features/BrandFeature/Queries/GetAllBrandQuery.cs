using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Product;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.BrandSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.BrandFeature.Queries
{
    public class GetAllBrandQuery : IRequest<PagedResponse<List<BrandDto>>>
    {
        public string? Parameter { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? Order { get; set; }
        public string? Column { get; set; }
        public bool All { get; set; }
    }

    public class GetAllBrandQueryHandler : IRequestHandler<GetAllBrandQuery, PagedResponse<List<BrandDto>>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<Brand> _repositoryAsync;

        public GetAllBrandQueryHandler(IMapper mapper, IRepositoryAsync<Brand> repositoryAsync)
        {
            _mapper = mapper;
            _repositoryAsync = repositoryAsync;
        }
        public async Task<PagedResponse<List<BrandDto>>> Handle(GetAllBrandQuery request, CancellationToken cancellationToken)
        {
            if (request.All)
            {
                request.PageNumber = 0;
                request.PageSize = await _repositoryAsync.CountAsync();
            }
            var brands = await _repositoryAsync.ListAsync(new FilterAndPaginationBrandSpecification(request.Parameter, request.PageNumber, request.PageSize, request.Order, request.Column));
            var dto = _mapper.Map<List<BrandDto>>(brands);
            return new PagedResponse<List<BrandDto>>(dto, request.PageNumber, request.PageSize, request.All ? request.PageSize : await _repositoryAsync.CountAsync());
        }
    }
}
