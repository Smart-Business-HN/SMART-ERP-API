using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Product;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.ProductDataSheetSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.ProductDataSheetFeature.Queries
{
    public class GetAllProductDataSheetsQuery : IRequest<Response<List<ProductDataSheetDto>>>
    {
        public class GetAllProductDataSheetsQueryHandler : IRequestHandler<GetAllProductDataSheetsQuery, Response<List<ProductDataSheetDto>>>
        {
            private readonly IMapper _mapper;
            private readonly IRepositoryAsync<ProductDataSheet> _repositoryAsync;

            public GetAllProductDataSheetsQueryHandler(IMapper mapper, IRepositoryAsync<ProductDataSheet> repositoryAsync)
            {
                _mapper = mapper;
                _repositoryAsync = repositoryAsync;
            }
            public async Task<Response<List<ProductDataSheetDto>>> Handle(GetAllProductDataSheetsQuery request, CancellationToken cancellationToken)
            {
                var productDataSheets = await _repositoryAsync.ListAsync(new ProductDataSheetIncludesSpecification(id: null, productId: null));
                var dto = _mapper.Map<List<ProductDataSheetDto>>(productDataSheets);
                return new Response<List<ProductDataSheetDto>>(dto);
            }
        }
    }
}
