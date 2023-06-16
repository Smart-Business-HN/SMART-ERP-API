using MediatR;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.QuoteProductFeature.Commands.DeleteQuoteProductCommand
{
    public class DeleteQuoteProductCommand : IRequest<Response<string>>
    {
        public int Id { get; set; }
    }

    public class DeleteQuoteProductCommandHandler : IRequestHandler<DeleteQuoteProductCommand, Response<string>>
    {
        private readonly IRepositoryAsync<QuoteProduct> _repositoryAsync;
        private readonly IRepositoryAsync<Product> _productRepositoryAsync;
        private readonly IRepositoryAsync<Opportunity> _cartRepositoryAsync;

        public DeleteQuoteProductCommandHandler(IRepositoryAsync<QuoteProduct> repositoryAsync, IRepositoryAsync<Product> productRepositoryAsync,
            IRepositoryAsync<Opportunity> cartRepositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
            _productRepositoryAsync = productRepositoryAsync;
            _cartRepositoryAsync = cartRepositoryAsync;
        }
        public async Task<Response<string>> Handle(DeleteQuoteProductCommand request, CancellationToken cancellationToken)
        {
            var quoteProduct = await _repositoryAsync.GetByIdAsync(request.Id);
            if (quoteProduct == null)
            {
                throw new KeyNotFoundException($"No se encontro ningun registro con el id {request.Id}");

            }
            var product = await _productRepositoryAsync.GetByIdAsync(quoteProduct!.ProductId);
            if (product == null)
            {
                throw new KeyNotFoundException($"No se encontro el producto con registro {quoteProduct!.ProductId}");
            }
            var cart = await _cartRepositoryAsync.GetByIdAsync(quoteProduct!.OpportunityId);
            if (cart == null)
            {
                throw new KeyNotFoundException($"No se encontro el carrito del registro {request.Id}");
            }
            cart.QtyItems -= quoteProduct.Quantity;
            cart.Total -= quoteProduct!.SalePrice * quoteProduct!.Quantity;
            await _cartRepositoryAsync.UpdateAsync(cart);
            await _cartRepositoryAsync.SaveChangesAsync();
            await _repositoryAsync.DeleteAsync(quoteProduct);
            await _repositoryAsync.SaveChangesAsync();
            return new Response<string>($"Producto eliminado correctamente", "Eliminado correctamente");
        }
    }
}
