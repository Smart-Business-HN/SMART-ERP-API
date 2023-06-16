using MediatR;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.BrandFeature.Commands.DeleteBrandCommand
{
    public class DeleteBrandCommand : IRequest<Response<string>>
    {
        public int Id { get; set; }
    }

    public class DeleteBrandCommandHandler : IRequestHandler<DeleteBrandCommand, Response<string>>
    {
        private readonly IRepositoryAsync<Brand> _repositoryAsync;

        public DeleteBrandCommandHandler(IRepositoryAsync<Brand> repositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
        }
        public async Task<Response<string>> Handle(DeleteBrandCommand request, CancellationToken cancellationToken)
        {
            var brand = await _repositoryAsync.GetByIdAsync(request.Id);
            if (brand == null)
            {
                throw new KeyNotFoundException($"No se encontro ningun registro con el id {request.Id}");
            }
            await _repositoryAsync.DeleteAsync(brand);
            await _repositoryAsync.SaveChangesAsync();
            return new Response<string>($"{brand.Name} eliminado correctamente", "Eliminado correctamente");
        }
    }
}
