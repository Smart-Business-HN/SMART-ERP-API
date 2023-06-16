using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Opportunity;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.QuoteProductSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.QuoteProductFeature.Queries
{
    public class GetQuoteProductByIdQuery : IRequest<Response<QuoteProductDto>>
    {
        public int Id { get; set; }
    }

    public class GetQuoteProductByIdQueryHandler : IRequestHandler<GetQuoteProductByIdQuery, Response<QuoteProductDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<QuoteProduct> _repositoryAsync;

        public GetQuoteProductByIdQueryHandler(IMapper mapper, IRepositoryAsync<QuoteProduct> repositoryAsync)
        {
            _mapper = mapper;
            _repositoryAsync = repositoryAsync;
        }

        public async Task<Response<QuoteProductDto>> Handle(GetQuoteProductByIdQuery request, CancellationToken cancellationToken)
        {
            var quoteProduct = await _repositoryAsync.FirstOrDefaultAsync(new QuoteProductIncludesSpecification(id: request.Id));
            if (quoteProduct == null)
            {
                throw new KeyNotFoundException($"Registro no encontrado con el id {request.Id}");
            }
            var dto = _mapper.Map<QuoteProductDto>(quoteProduct);
            return new Response<QuoteProductDto>(dto);
        }
    }
}
