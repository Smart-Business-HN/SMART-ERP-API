using MediatR;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.ProductFtrFeature.Commands.DeleteProductFtrCommand
{
    public class DeleteProductFtrCommand : IRequest<Response<string>>
    {
        public int Id { get; set; }
    }

    public class DeleteProductFtrCommandHandler : IRequestHandler<DeleteProductFtrCommand, Response<string>>
    {
        private readonly IRepositoryAsync<ProductFeature> _repositoryAsync;

        public DeleteProductFtrCommandHandler(IRepositoryAsync<ProductFeature> repositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
        }
        public async Task<Response<string>> Handle(DeleteProductFtrCommand request, CancellationToken cancellationToken)
        {
            var productFeature = await _repositoryAsync.GetByIdAsync(request.Id);
            if (productFeature == null)
            {
                throw new KeyNotFoundException($"No se encontro ningun registro con el id {request.Id}");
            }
            await _repositoryAsync.DeleteAsync(productFeature);
            await _repositoryAsync.SaveChangesAsync();
            return new Response<string>($"{productFeature.Title} eliminado correctamente", "Eliminado correctamente");
        }
    }
}
