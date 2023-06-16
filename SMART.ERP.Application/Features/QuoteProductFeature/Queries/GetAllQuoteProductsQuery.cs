using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Opportunity;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.QuoteProductSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.QuoteProductFeature.Queries
{
    public class GetAllQuoteProductsQuery : IRequest<Response<List<QuoteProductDto>>>
    {
        public class GetAllQuoteProductsQueryHandler : IRequestHandler<GetAllQuoteProductsQuery, Response<List<QuoteProductDto>>>
        {
            private readonly IMapper _mapper;
            private readonly IRepositoryAsync<QuoteProduct> _repositoryAsync;

            public GetAllQuoteProductsQueryHandler(IMapper mapper, IRepositoryAsync<QuoteProduct> repositoryAsync)
            {
                _mapper = mapper;
                _repositoryAsync = repositoryAsync;
            }
            public async Task<Response<List<QuoteProductDto>>> Handle(GetAllQuoteProductsQuery request, CancellationToken cancellationToken)
            {
                var quoteProducts = await _repositoryAsync.ListAsync(new QuoteProductIncludesSpecification(id: null));
                var dto = _mapper.Map<List<QuoteProductDto>>(quoteProducts);
                return new Response<List<QuoteProductDto>>(dto);
            }
        }
    }
}
