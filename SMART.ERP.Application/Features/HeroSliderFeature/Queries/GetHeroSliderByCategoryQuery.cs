using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Company;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.HeroSliderSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.HeroSliderFeature.Queries
{
    public class GetHeroSliderByCategoryQuery : IRequest<Response<List<CategoryWithHeroSliderDto>>>
    {
        public class GetHeroSliderByCategoryQueryHandler : IRequestHandler<GetHeroSliderByCategoryQuery, Response<List<CategoryWithHeroSliderDto>>>
        {
            private readonly IMapper _mapper;
            private readonly IRepositoryAsync<Category> _repositoryAsync;

            public GetHeroSliderByCategoryQueryHandler(IMapper mapper, IRepositoryAsync<Category> repositoryAsync)
            {
                _mapper = mapper;
                _repositoryAsync = repositoryAsync;
            }
            public async Task<Response<List<CategoryWithHeroSliderDto>>> Handle(GetHeroSliderByCategoryQuery request, CancellationToken cancellationToken)
            {
                var categories = await _repositoryAsync.ListAsync(new IncludeHeroSliderSpecification(0));
                var dto = _mapper.Map<List<CategoryWithHeroSliderDto>>(categories);
                return new Response<List<CategoryWithHeroSliderDto>>(dto);
            }
        }
    }
}
