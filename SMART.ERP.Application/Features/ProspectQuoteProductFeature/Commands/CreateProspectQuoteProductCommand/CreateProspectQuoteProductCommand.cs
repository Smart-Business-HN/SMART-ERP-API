using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.ProspectQuoteProduct;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.ProspectQuoteProductFeature.Commands.CreateProspectQuoteProductCommand
{
    public class CreateProspectQuoteProductCommand : IRequest<Response<List<ProspectQuoteProductDto>>>
    {
        public Guid ProspectId { get; set; }
        public List<int> QuoteProducts { get; set; } = null!;
    }

    public class CreateProspectQuoteProductCommandHandler : IRequestHandler<CreateProspectQuoteProductCommand, Response<List<ProspectQuoteProductDto>>>
    {
        private readonly IRepositoryAsync<ProspectQuoteProduct> _repositoryAsync;
        private readonly IRepositoryAsync<Prospect> _prospectRepositoryAsync;
        private readonly IRepositoryAsync<Product> _productRepositoryAsync;
        private readonly IMapper _mapper;

        public CreateProspectQuoteProductCommandHandler(IRepositoryAsync<ProspectQuoteProduct> repositoryAsync, IRepositoryAsync<Prospect> prospectRepositoryAsync,
            IRepositoryAsync<Product> productRepositoryAsync, IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _prospectRepositoryAsync = prospectRepositoryAsync;
            _productRepositoryAsync = productRepositoryAsync;
            _mapper = mapper;
        }

        public async Task<Response<List<ProspectQuoteProductDto>>> Handle(CreateProspectQuoteProductCommand request, CancellationToken cancellationToken)
        {
            var checkProspect = await _prospectRepositoryAsync.GetByIdAsync(request.ProspectId);
            if (checkProspect == null)
            {
                throw new KeyNotFoundException($"No se encontro el prospecto con Id {request.ProspectId}");
            }
            var products = await _productRepositoryAsync.ListAsync();
            var quoteList = new List<ProspectQuoteProduct>();
            foreach (var quote in request.QuoteProducts)
            {
                var checkProduct = products.FirstOrDefault(x => x.Id == quote);
                if (checkProduct == null)
                {
                    throw new KeyNotFoundException($"No se encontro el producto con id {quote}");
                }
                var newRecord = new ProspectQuoteProduct
                {
                    ProspectId = request.ProspectId,
                    ProductId = quote,
                    IsActive = true
                };
                quoteList.Add(newRecord);
            }
            var response = await _repositoryAsync.AddRangeAsync(quoteList);
            await _repositoryAsync.SaveChangesAsync();
            var dto = _mapper.Map<List<ProspectQuoteProductDto>>(response);
            return new Response<List<ProspectQuoteProductDto>>(dto, "Productos agregados exitosamente");
        }
    }
}
