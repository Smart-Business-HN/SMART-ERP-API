using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Product;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.SubcategorySpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.SubcategoryFeature.Queries
{
    public class GetAllSubcategoriesQuery : IRequest<PagedResponse<List<SubcategoryDto>>>
    {
        public string? Parameter { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? Order { get; set; }
        public string? Column { get; set; }
        public bool All { get; set; }

        public class GetAllSubcategoriesQueryHandler : IRequestHandler<GetAllSubcategoriesQuery, PagedResponse<List<SubcategoryDto>>>
        {
            private readonly IMapper _mapper;
            private readonly IRepositoryAsync<Subcategory> _repositoryAsync;
            private readonly IRepositoryAsync<Category> _categoryRepositoryAsync;

            public GetAllSubcategoriesQueryHandler(IMapper mapper, IRepositoryAsync<Subcategory> repositoryAsync,
                IRepositoryAsync<Category> categoryRepositoryAsync)
            {
                _mapper = mapper;
                _repositoryAsync = repositoryAsync;
                _categoryRepositoryAsync = categoryRepositoryAsync;
            }

            public async Task<PagedResponse<List<SubcategoryDto>>> Handle(GetAllSubcategoriesQuery request, CancellationToken cancellationToken)
            {

                if (request.All)
                {
                    request.PageNumber = 0;
                    request.PageSize = await _repositoryAsync.CountAsync();
                }

                var subcategories = await _repositoryAsync.ListAsync(
                    new FilterAndPaginationSubcategorySpecification(request.Parameter, request.PageNumber, request.PageSize, request.Order, request.Column));
                var categories = await _categoryRepositoryAsync.ListAsync();
                var dto = _mapper.Map<List<SubcategoryDto>>(subcategories);
                for (int i = 0; i < dto.Count(); i++)
                {
                    var category = categories.Find(x => x.Id == dto[i].CategoryId);
                    if (category != null)
                    {
                        var categoryDto = _mapper.Map<CategoryDto>(category);
                        dto[i].Category = categoryDto;
                    }
                }
                return new PagedResponse<List<SubcategoryDto>>(dto, request.PageNumber, request.PageSize, request.All ? request.PageSize : await _repositoryAsync.CountAsync());
            }
        }
    }
}
