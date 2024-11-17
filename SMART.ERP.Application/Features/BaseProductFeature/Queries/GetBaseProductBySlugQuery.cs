using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Product;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.JwtService;
using SMART.ERP.Application.Specifications.ProductSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.BaseProductFeature.Queries
{
    public class GetBaseProductBySlugQuery : IRequest<Response<ProductDto>>
    {
        public string Slug { get; set; } = null!;
        public bool? IsLogged { get; set; }
        public int? CustomerTypeId { get; set; }
    }
    public class GetBaseProductBySlugQueryHandler : IRequestHandler<GetBaseProductBySlugQuery, Response<ProductDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<Product> _repositoryAsync;
        private readonly IJwtService _jwtService;

        public GetBaseProductBySlugQueryHandler(IMapper mapper, IRepositoryAsync<Product> repositoryAsync, IJwtService jwtService)
        {
            _mapper = mapper;
            _repositoryAsync = repositoryAsync;
            _jwtService = jwtService;
        }

        public async Task<Response<ProductDto>> Handle(GetBaseProductBySlugQuery request, CancellationToken cancellationToken)
        {
            var product = await _repositoryAsync.FirstOrDefaultAsync(new ProductIncludesSpecification(id: null, slug: request.Slug));
            if (product == null)
            {
                throw new KeyNotFoundException($"Registro no encontrado con el slug {request.Slug}");
            }
            if (request.IsLogged.HasValue && request.IsLogged.Value)
            {
                product.RecomendedSalePrice = Math.Ceiling((product.CostPrice * (decimal)1.2) * (1 + (product.Tax!.Rate / 100)));
            }
            else
            {
                product.RecomendedSalePrice = Math.Ceiling((product.CostPrice * (decimal)1.3) * (1 + (product.Tax!.Rate / 100)));
            }
            product.Tax = null;
            product.CostPrice = 0;

            var dto = _mapper.Map<ProductDto>(product);
            return new Response<ProductDto>(dto);
        }
    }
}
