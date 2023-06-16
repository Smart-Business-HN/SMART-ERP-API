using MediatR;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.ProductDataSheetSpecification;
using SMART.ERP.Application.Specifications.ProductFeatureSpecification;
using SMART.ERP.Application.Specifications.ProductImageSpecification;
using SMART.ERP.Application.Specifications.ProductSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.BaseProductFeature.Commands.DeleteBaseProductCommand
{
    public class DeleteBaseProductCommand : IRequest<Response<string>>
    {
        public int Id { get; set; }
    }

    public class DeleteBaseProductCommandHandler : IRequestHandler<DeleteBaseProductCommand, Response<string>>
    {
        private readonly IRepositoryAsync<Product> _repositoryAsync;
        private readonly IRepositoryAsync<ProductDataSheet> _productDataSheetRepositoryAsync;
        private readonly IRepositoryAsync<ProductFeature> _productFeatureRepositoryAsync;
        private readonly IRepositoryAsync<ProductImage> _productImageRepositoryAsync;

        public DeleteBaseProductCommandHandler(IRepositoryAsync<Product> repositoryAsync,
            IRepositoryAsync<ProductDataSheet> productDataSheetRepositoryAsync,
            IRepositoryAsync<ProductFeature> productFeatureRepositoryAsync,
            IRepositoryAsync<ProductImage> productImageRepositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
            _productDataSheetRepositoryAsync = productDataSheetRepositoryAsync;
            _productFeatureRepositoryAsync = productFeatureRepositoryAsync;
            _productImageRepositoryAsync = productImageRepositoryAsync;
        }
        public async Task<Response<string>> Handle(DeleteBaseProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _repositoryAsync.GetByIdAsync(request.Id);
            if (product == null)
            {
                throw new KeyNotFoundException($"No se encontro ningun registro con el id {request.Id}");
            }
            await DeleteItems(request.Id);
            await _repositoryAsync.DeleteAsync(product);
            await _repositoryAsync.SaveChangesAsync();
            return new Response<string>($"{product.Name} eliminado correctamente", "Eliminado correctamente");
        }

        public async Task<bool> DeleteItems(int prouctId)
        {
            var productDataSheets = await _productDataSheetRepositoryAsync.ListAsync(new ProductDataSheetIncludesSpecification(null, prouctId));
            foreach (var productDataSheet in productDataSheets)
            {
                await _productDataSheetRepositoryAsync.DeleteAsync(productDataSheet);
                await _productDataSheetRepositoryAsync.SaveChangesAsync();
            }
            var productFeatures = await _productFeatureRepositoryAsync.ListAsync(new ProductFeatureByProjectSpecification(prouctId));
            foreach (var productFeature in productFeatures)
            {
                await _productFeatureRepositoryAsync.DeleteAsync(productFeature);
                await _productFeatureRepositoryAsync.SaveChangesAsync();
            }
            var productImages = await _productImageRepositoryAsync.ListAsync(new ProductImageByProjectSpecification(prouctId));
            foreach (var productImage in productImages)
            {
                await _productImageRepositoryAsync.DeleteAsync(productImage);
                await _productImageRepositoryAsync.SaveChangesAsync();
            }
            return true;

        }
    }
}
