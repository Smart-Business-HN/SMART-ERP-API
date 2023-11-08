using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Product;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.ProductSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.BaseProductFeature.Queries
{
    public class GetProductsBySameCategorySlugQuery : IRequest<Response<List<ResumeProductDto>>>
    {
        public string CategorySlug { get; set; } = null!;
        public string ProductSlug { get; set; } = null!;
    }

    public class GetProductsBySameCategorySlugQueryHandler : IRequestHandler<GetProductsBySameCategorySlugQuery, Response<List<ResumeProductDto>>>
    {
        private readonly IRepositoryAsync<Product> _repositoryAsync;
        private readonly IMapper _mapper;

        public GetProductsBySameCategorySlugQueryHandler(IRepositoryAsync<Product> repositoryAsync, IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _mapper = mapper;
        }

        public async Task<Response<List<ResumeProductDto>>> Handle(GetProductsBySameCategorySlugQuery request, CancellationToken cancellationToken)
        {
            var products = await _repositoryAsync.ListAsync(new FilterProductsByCategorySlugAndProductSlugSpecification(request.CategorySlug, request.ProductSlug));
            var dto = _mapper.Map<List<ResumeProductDto>>(products);
            var producsToShow = SelectRandomElements(dto, 4);
            return new Response<List<ResumeProductDto>>(producsToShow);
        }
        static List<ResumeProductDto> SelectRandomElements(List<ResumeProductDto> inputArray, int count)
        {
            Random random = new Random();
            List<ResumeProductDto> resultArray = inputArray.OrderBy(_ => random.Next()).Take(count).ToList();

            return resultArray;
        }
    }
}
