using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Product;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.ProductSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.BaseProductFeature.Queries
{
    public class GetAllProductByCategoryQuery : IRequest<Response<List<ResumeProductDto>>>
    {
        public int CategoryId { get; set; }
    }

    public class GetAllProductByCategoryQueryHandler : IRequestHandler<GetAllProductByCategoryQuery, Response<List<ResumeProductDto>>>
    {
        private readonly IRepositoryAsync<Product> _repositoryAsync;
        private readonly IMapper _mapper;

        public GetAllProductByCategoryQueryHandler(IRepositoryAsync<Product> repositoryAsync, IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _mapper = mapper;
        }

        public async Task<Response<List<ResumeProductDto>>> Handle(GetAllProductByCategoryQuery request, CancellationToken cancellationToken)
        {
            var products = await _repositoryAsync.ListAsync(new FilterProductCategorySpecification(request.CategoryId));
            var dto = _mapper.Map<List<ResumeProductDto>>(products);
            return new Response<List<ResumeProductDto>>(dto);
        }
    }
}
