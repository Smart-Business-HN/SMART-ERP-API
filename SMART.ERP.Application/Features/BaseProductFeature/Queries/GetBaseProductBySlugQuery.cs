using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Product;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.ProductSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.ERP.Application.Features.BaseProductFeature.Queries
{
    public class GetBaseProductBySlugQuery : IRequest<Response<ProductDto>>
    {
        public string Slug { get; set; }
    }
    public class GetBaseProductBySlugQueryHandler : IRequestHandler<GetBaseProductBySlugQuery, Response<ProductDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<Product> _repositoryAsync;

        public GetBaseProductBySlugQueryHandler(IMapper mapper, IRepositoryAsync<Product> repositoryAsync)
        {
            _mapper = mapper;
            _repositoryAsync = repositoryAsync;
        }

        public async Task<Response<ProductDto>> Handle(GetBaseProductBySlugQuery request, CancellationToken cancellationToken)
        {
            var product = await _repositoryAsync.FirstOrDefaultAsync(new ProductIncludesSpecification(id:null,slug: request.Slug));
            if (product == null)
            {
                throw new KeyNotFoundException($"Registro no encontrado con el slug {request.Slug}");
            }
            var dto = _mapper.Map<ProductDto>(product);
            return new Response<ProductDto>(dto);
        }
    }
}
