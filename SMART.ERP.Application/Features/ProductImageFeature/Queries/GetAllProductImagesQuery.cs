using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Product;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.ProductImageFeature.Queries
{
    public class GetAllProductImagesQuery : IRequest<Response<List<ProductImageDto>>>
    {
        public class GetAllProductImagesQueryHandler : IRequestHandler<GetAllProductImagesQuery, Response<List<ProductImageDto>>>
        {
            private readonly IMapper _mapper;
            private readonly IRepositoryAsync<ProductImage> _repositoryAsync;

            public GetAllProductImagesQueryHandler(IMapper mapper, IRepositoryAsync<ProductImage> repositoryAsync)
            {
                _mapper = mapper;
                _repositoryAsync = repositoryAsync;
            }
            public async Task<Response<List<ProductImageDto>>> Handle(GetAllProductImagesQuery request, CancellationToken cancellationToken)
            {
                var productImages = await _repositoryAsync.ListAsync();
                var dto = _mapper.Map<List<ProductImageDto>>(productImages);
                return new Response<List<ProductImageDto>>(dto);
            }
        }
    }
}
