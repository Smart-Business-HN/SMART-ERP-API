using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Product;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.ProductSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.BaseProductFeature.Queries
{
    public class GetBasicDetailProductById : IRequest<Response<BasicDetailProductDto>>
    {
        public int Id { get; set; }
    }

    public class GetBasicDetailProductByIdHandler : IRequestHandler<GetBasicDetailProductById, Response<BasicDetailProductDto>>
    {
        private readonly IRepositoryAsync<Product> _repositoryAsync;
        private readonly IMapper _mapper;

        public GetBasicDetailProductByIdHandler(IRepositoryAsync<Product> repositoryAsync, IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _mapper = mapper;
        }

        public async Task<Response<BasicDetailProductDto>> Handle(GetBasicDetailProductById request, CancellationToken cancellationToken)
        {
            var product = await _repositoryAsync.FirstOrDefaultAsync(new ProductIncludesSpecification(request.Id));
            if (product == null)
            {
                throw new KeyNotFoundException($"No se encontro el registro con id {request.Id}");
            }
            var dto = _mapper.Map<BasicDetailProductDto>(product);
            return new Response<BasicDetailProductDto>(dto);
        }
    }
}
