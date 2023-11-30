using MediatR;
using SMART.ERP.Application.DTOs.Product;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.CategorySpecification;
using SMART.ERP.Application.Specifications.ProductSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.CategoryFeature.Queries
{
    public class GetAllNavCategoriesQuery : IRequest<Response<List<NavCategoryDto>>>
    {
        public class GetAllNavCategoriesQueryHandler : IRequestHandler<GetAllNavCategoriesQuery, Response<List<NavCategoryDto>>>
        {
            private readonly IRepositoryAsync<Category> _repositoryAsync;
            private readonly IRepositoryAsync<Product> _productRepositoryAsync;

            public GetAllNavCategoriesQueryHandler(IRepositoryAsync<Category> repositoryAsync,
                IRepositoryAsync<Product> productRepositoryAsync)
            {
                _repositoryAsync = repositoryAsync;
                _productRepositoryAsync = productRepositoryAsync;
            }
            public async Task<Response<List<NavCategoryDto>>> Handle(GetAllNavCategoriesQuery request, CancellationToken cancellationToken)
            {
                var categories = await _repositoryAsync.ListAsync(new NavCategorySpecification());
                List<NavCategoryDto> navs = new List<NavCategoryDto>();
                foreach (var item in categories)
                {
                    NavCategoryDto nav = new NavCategoryDto();
                    List<ResumeSubcategoryDto> subcategories = new List<ResumeSubcategoryDto>();
                    nav.Id = item.Id;
                    nav.Category = item.Name;
                    nav.Slug = item.Slug;
                    foreach (var detail in item.Subcategories)
                    {
                        if (await _productRepositoryAsync.AnyAsync(new FilterNavSpecification(detail.Id)))
                        {
                            ResumeSubcategoryDto subcategory = new ResumeSubcategoryDto();
                            subcategory.Name = detail.Name;
                            subcategory.Slug = detail.Slug;
                            subcategory.Id = detail.Id;
                            subcategories.Add(subcategory);
                        }
                    }
                    nav.SubCategories = subcategories;
                    navs.Add(nav);
                }
                return new Response<List<NavCategoryDto>>(navs);
            }
        }
    }
}
