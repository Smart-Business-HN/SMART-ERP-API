using MediatR;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.CategoryFeature.Commands.DeleteCategoryCommand
{
    public class DeleteCategoryCommand : IRequest<Response<string>>
    {
        public int Id { get; set; }
    }

    public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, Response<string>>
    {
        private readonly IRepositoryAsync<Category> _repositoryAsync;

        public DeleteCategoryCommandHandler(IRepositoryAsync<Category> repositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
        }
        public async Task<Response<string>> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = await _repositoryAsync.GetByIdAsync(request.Id);
            if (category == null)
            {
                throw new KeyNotFoundException($"No se encontro ningun registro con el id {request.Id}");
            }
            await _repositoryAsync.DeleteAsync(category);
            await _repositoryAsync.SaveChangesAsync();
            return new Response<string>($"{category.Name} eliminado correctamente", "Eliminado correctamente");
        }
    }
}
