using MediatR;
using SMART.ERP.Application.DTOs.Product;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.CategorySpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.CategoryFeature.Queries
{
    public class GetAllCategoriesNotPartsQuery : IRequest<Response<List<SubcategoryDto>>>
    {
        public class GetAllCategoriesNotPartsQueryHandler : IRequestHandler<GetAllCategoriesNotPartsQuery, Response<List<SubcategoryDto>>>
        {
            private readonly IRepositoryAsync<Category> _repositoryAsync;

            public GetAllCategoriesNotPartsQueryHandler(IRepositoryAsync<Category> repositoryAsync)
            {
                _repositoryAsync = repositoryAsync;
            }
            public async Task<Response<List<SubcategoryDto>>> Handle(GetAllCategoriesNotPartsQuery request, CancellationToken cancellationToken)
            {
                List<SubcategoryDto> subcategories = new List<SubcategoryDto>();
                var categories = await _repositoryAsync.ListAsync(new GetCategoriesNotPartsSpecification());
                if (categories.Count > 0)
                {
                    foreach (var category in categories)
                    {
                        if (category.Subcategories.Count > 0)
                        {
                            foreach (var subcategory in category.Subcategories)
                            {
                                var item = new SubcategoryDto()
                                {
                                    Id = subcategory.Id,
                                    Name = subcategory.Name,
                                    CategoryId = subcategory.CategoryId,
                                    IsActive = subcategory.IsActive,
                                };
                                subcategories.Add(item);
                            }
                        }
                    }
                }

                return new Response<List<SubcategoryDto>>(subcategories);
            }
        }
    }
}
