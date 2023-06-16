using MediatR;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.ProductDataSheetFeature.Commands.DeleteProductDataSheetCommand
{
    public class DeleteProductDataSheetCommand : IRequest<Response<string>>
    {
        public int Id { get; set; }
    }

    public class DeleteProductDataSheetCommandHandler : IRequestHandler<DeleteProductDataSheetCommand, Response<string>>
    {
        private readonly IRepositoryAsync<ProductDataSheet> _repositoryAsync;

        public DeleteProductDataSheetCommandHandler(IRepositoryAsync<ProductDataSheet> repositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
        }
        public async Task<Response<string>> Handle(DeleteProductDataSheetCommand request, CancellationToken cancellationToken)
        {
            var productDataSheet = await _repositoryAsync.GetByIdAsync(request.Id);
            if (productDataSheet == null)
            {
                throw new KeyNotFoundException($"No se encontro ningun registro con el id {request.Id}");
            }
            await _repositoryAsync.DeleteAsync(productDataSheet);
            await _repositoryAsync.SaveChangesAsync();
            return new Response<string>($"{productDataSheet.Title} eliminado correctamente", "Eliminado correctamente");
        }
    }
}
