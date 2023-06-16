using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Product;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.ProductImageFeature.Queries
{
    public class GetProductImageByIdQuery : IRequest<Response<ProductImageDto>>
    {
        public int Id { get; set; }
    }

    public class GetProductImageByIdQueryHandler : IRequestHandler<GetProductImageByIdQuery, Response<ProductImageDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<ProductImage> _repositoryAsync;

        public GetProductImageByIdQueryHandler(IMapper mapper, IRepositoryAsync<ProductImage> repositoryAsync)
        {
            _mapper = mapper;
            _repositoryAsync = repositoryAsync;
        }

        public async Task<Response<ProductImageDto>> Handle(GetProductImageByIdQuery request, CancellationToken cancellationToken)
        {
            var productImage = await _repositoryAsync.GetByIdAsync(request.Id);
            if (productImage == null)
            {
                throw new KeyNotFoundException($"Registro no encontrado con el id {request.Id}");
            }
            var dto = _mapper.Map<ProductImageDto>(productImage);
            return new Response<ProductImageDto>(dto);
        }
    }
}
