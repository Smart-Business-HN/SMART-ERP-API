using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Product;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.ProductDataSheetSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.ProductDataSheetFeature.Queries
{
    public class GetProductDataSheetByIdQuery : IRequest<Response<ProductDataSheetDto>>
    {
        public int Id { get; set; }
    }

    public class GetProductDataSheetByIdQueryHandler : IRequestHandler<GetProductDataSheetByIdQuery, Response<ProductDataSheetDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<ProductDataSheet> _repositoryAsync;

        public GetProductDataSheetByIdQueryHandler(IMapper mapper, IRepositoryAsync<ProductDataSheet> repositoryAsync)
        {
            _mapper = mapper;
            _repositoryAsync = repositoryAsync;
        }

        public async Task<Response<ProductDataSheetDto>> Handle(GetProductDataSheetByIdQuery request, CancellationToken cancellationToken)
        {
            var productDataSheet = await _repositoryAsync.FirstOrDefaultAsync(new ProductDataSheetIncludesSpecification(id: request.Id, productId: null));
            if (productDataSheet == null)
            {
                throw new KeyNotFoundException($"Registro no encontrado con el id {request.Id}");
            }
            var dto = _mapper.Map<ProductDataSheetDto>(productDataSheet);
            return new Response<ProductDataSheetDto>(dto);
        }
    }
}
