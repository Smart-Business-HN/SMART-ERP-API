using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Product;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.ProductFtrFeature.Queries
{
    public class GetAllProductFtrsQuery : IRequest<Response<List<ProductFeatureDto>>>
    {
        public class GetAllProductFtrFeatureQueryHandler : IRequestHandler<GetAllProductFtrsQuery, Response<List<ProductFeatureDto>>>
        {
            private readonly IMapper _mapper;
            private readonly IRepositoryAsync<ProductFeature> _repositoryAsync;

            public GetAllProductFtrFeatureQueryHandler(IMapper mapper, IRepositoryAsync<ProductFeature> repositoryAsync)
            {
                _mapper = mapper;
                _repositoryAsync = repositoryAsync;
            }
            public async Task<Response<List<ProductFeatureDto>>> Handle(GetAllProductFtrsQuery request, CancellationToken cancellationToken)
            {
                var productFeatures = await _repositoryAsync.ListAsync();
                var dto = _mapper.Map<List<ProductFeatureDto>>(productFeatures);
                return new Response<List<ProductFeatureDto>>(dto);
            }
        }
    }
}
