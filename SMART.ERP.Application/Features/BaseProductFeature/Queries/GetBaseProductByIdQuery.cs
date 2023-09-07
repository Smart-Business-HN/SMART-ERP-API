using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Product;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.ProductSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.BaseProductFeature.Queries
{
    public class GetBaseProductByIdQuery : IRequest<Response<ProductDto>>
    {
        public int Id { get; set; }
    }
    public class GetBaseProductByIdQueryHandler : IRequestHandler<GetBaseProductByIdQuery, Response<ProductDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<Product> _repositoryAsync;

        public GetBaseProductByIdQueryHandler(IMapper mapper, IRepositoryAsync<Product> repositoryAsync)
        {
            _mapper = mapper;
            _repositoryAsync = repositoryAsync;
        }

        public async Task<Response<ProductDto>> Handle(GetBaseProductByIdQuery request, CancellationToken cancellationToken)
        {
            var product = await _repositoryAsync.FirstOrDefaultAsync(new ProductIncludesSpecification(id: request.Id,slug:null));
            if (product == null)
            {
                throw new KeyNotFoundException($"Registro no encontrado con el id {request.Id}");
            }
            var dto = _mapper.Map<ProductDto>(product);
            return new Response<ProductDto>(dto);
        }
    }
}
