using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Product;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.ProductSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.BaseProductFeature.Queries
{
    public class GetAllProductBySubcategoryQuery : IRequest<Response<List<ResumeProductDto>>>
    {
        public int SubCategoryId { get; set; }
    }
    public class GetAllProductBySubcategoryQueryHandler : IRequestHandler<GetAllProductBySubcategoryQuery, Response<List<ResumeProductDto>>>
    {
        private readonly IRepositoryAsync<Product> _repositoryAsync;
        private readonly IMapper _mapper;

        public GetAllProductBySubcategoryQueryHandler(IRepositoryAsync<Product> repositoryAsync, IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _mapper = mapper;
        }

        public async Task<Response<List<ResumeProductDto>>> Handle(GetAllProductBySubcategoryQuery request, CancellationToken cancellationToken)
        {
            var products = await _repositoryAsync.ListAsync(new FilterProductSubcategorySpecification(request.SubCategoryId));
            var dto = _mapper.Map<List<ResumeProductDto>>(products);
            return new Response<List<ResumeProductDto>>(dto);
        }
    }

}
