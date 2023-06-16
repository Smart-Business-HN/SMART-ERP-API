using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Product;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.ProductFtrFeature.Queries
{
    public class GetProductFtrByIdQuery : IRequest<Response<ProductFeatureDto>>
    {
        public int Id { get; set; }
    }

    public class GetProductFtrByIdQueryHandler : IRequestHandler<GetProductFtrByIdQuery, Response<ProductFeatureDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<ProductFeature> _repositoryAsync;

        public GetProductFtrByIdQueryHandler(IMapper mapper, IRepositoryAsync<ProductFeature> repositoryAsync)
        {
            _mapper = mapper;
            _repositoryAsync = repositoryAsync;
        }

        public async Task<Response<ProductFeatureDto>> Handle(GetProductFtrByIdQuery request, CancellationToken cancellationToken)
        {
            var productFeature = await _repositoryAsync.GetByIdAsync(request.Id);
            if (productFeature == null)
            {
                throw new KeyNotFoundException($"Registro no encontrado con el id {request.Id}");
            }
            var dto = _mapper.Map<ProductFeatureDto>(productFeature);
            return new Response<ProductFeatureDto>(dto);
        }
    }
}
