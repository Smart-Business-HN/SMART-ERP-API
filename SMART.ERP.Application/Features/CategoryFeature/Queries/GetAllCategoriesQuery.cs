using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Product;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.CategorySpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.CategoryFeature.Queries
{
    public class GetAllCategoriesQuery : IRequest<PagedResponse<List<CategoryDto>>>
    {
        public string? Parameter { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? Order { get; set; }
        public string? Column { get; set; }
        public bool All { get; set; }

        public class GetAllCategoriesQueryHandler : IRequestHandler<GetAllCategoriesQuery, PagedResponse<List<CategoryDto>>>
        {
            private readonly IMapper _mapper;
            private readonly IRepositoryAsync<Category> _repositoryAsync;

            public GetAllCategoriesQueryHandler(IMapper mapper, IRepositoryAsync<Category> repositoryAsync)
            {
                _mapper = mapper;
                _repositoryAsync = repositoryAsync;
            }
            public async Task<PagedResponse<List<CategoryDto>>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
            {

                if (request.All)
                {
                    request.PageNumber = 0;
                    request.PageSize = await _repositoryAsync.CountAsync();
                }

                var categories = await _repositoryAsync.ListAsync(
                    new FilterAndPaginationCategorySpecification(request.Parameter, request.PageNumber, request.PageSize, request.Order, request.Column));
                var dto = _mapper.Map<List<CategoryDto>>(categories);
                return new PagedResponse<List<CategoryDto>>(dto, request.PageNumber, request.PageSize, request.All ? request.PageSize : await _repositoryAsync.CountAsync());
            }
        }
    }
}
