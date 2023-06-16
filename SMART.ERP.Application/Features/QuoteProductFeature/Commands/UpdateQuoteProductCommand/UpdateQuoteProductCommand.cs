using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Opportunity;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.QuoteProductFeature.Commands.UpdateQuoteProductCommand
{
    public class UpdateQuoteProductCommand : IRequest<Response<QuoteProductDto>>
    {
        public int Id { get; set; }
        public decimal SalePrice { get; set; }
        public int Quantity { get; set; }
    }

    public class UpdateQuoteProductCommandHandler : IRequestHandler<UpdateQuoteProductCommand, Response<QuoteProductDto>>
    {
        private readonly IRepositoryAsync<QuoteProduct> _repositoryAsync;
        private readonly IRepositoryAsync<Opportunity> _opportunityRepositoryAsync;
        private readonly IRepositoryAsync<Product> _productRepositoryAsync;
        private readonly IMapper _mapper;

        public UpdateQuoteProductCommandHandler(IMapper mapper, IRepositoryAsync<QuoteProduct> repositoryAsync,
            IRepositoryAsync<Opportunity> opportunityRepositoryAsync, IRepositoryAsync<Product> productRepositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
            _opportunityRepositoryAsync = opportunityRepositoryAsync;
            _productRepositoryAsync = productRepositoryAsync;
            _mapper = mapper;
        }
        public async Task<Response<QuoteProductDto>> Handle(UpdateQuoteProductCommand request, CancellationToken cancellationToken)
        {
            var quoteProduct = await _repositoryAsync.GetByIdAsync(request.Id);
            if (quoteProduct == null)
            {
                throw new KeyNotFoundException($"No se encontro ningun registro con el id {request.Id}");
            }
            var product = await _productRepositoryAsync.GetByIdAsync(quoteProduct!.ProductId);
            if (product == null)
            {
                throw new KeyNotFoundException($"No se encontro ningun producto con el id {quoteProduct!.ProductId}");
            }
            var opportunity = await _opportunityRepositoryAsync.GetByIdAsync(quoteProduct!.OpportunityId);
            if (opportunity == null)
            {
                throw new KeyNotFoundException($"No se encontro ninguna oportunidad con el id {quoteProduct!.OpportunityId}");
            }
            if (quoteProduct!.Quantity == request.Quantity && quoteProduct!.SalePrice == request.SalePrice)
            {
                throw new ApiException("No se ha detectado ningun cambio");
            }
            var original = quoteProduct!.Quantity * quoteProduct!.SalePrice;
            var result = request.Quantity * request.SalePrice;
            if (original > result)
            {
                opportunity.Total -= original - result;
            }
            else if (original < result)
            {
                opportunity.Total += result - original;
            }
            if (request.Quantity != quoteProduct.Quantity)
            {
                if (request.Quantity < quoteProduct.Quantity)
                {
                    opportunity.QtyItems -= quoteProduct.Quantity - request.Quantity;
                }
                else
                {
                    opportunity.QtyItems += request.Quantity - quoteProduct.Quantity;
                }
            }
            await _opportunityRepositoryAsync.UpdateAsync(opportunity);
            await _opportunityRepositoryAsync.SaveChangesAsync();
            quoteProduct!.Quantity = request.Quantity;
            quoteProduct!.Product = product;
            quoteProduct!.SalePrice = request.SalePrice;
            await _repositoryAsync.UpdateAsync(quoteProduct);
            await _repositoryAsync.SaveChangesAsync();
            var dto = _mapper.Map<QuoteProductDto>(quoteProduct);
            return new Response<QuoteProductDto>(dto, message: $"Item actualizado correctamente");
        }
    }
}
