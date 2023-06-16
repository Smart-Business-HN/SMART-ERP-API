using MediatR;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.SubcategoryFeature.Commands.DeleteSubcategoryCommand
{
    public class DeleteSubcategoryCommand : IRequest<Response<string>>
    {
        public int Id { get; set; }
    }

    public class DeleteSubcategoryCommandHandler : IRequestHandler<DeleteSubcategoryCommand, Response<string>>
    {
        private readonly IRepositoryAsync<Subcategory> _repositoryAsync;

        public DeleteSubcategoryCommandHandler(IRepositoryAsync<Subcategory> repositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
        }
        public async Task<Response<string>> Handle(DeleteSubcategoryCommand request, CancellationToken cancellationToken)
        {
            var subcategory = await _repositoryAsync.GetByIdAsync(request.Id);
            if (subcategory == null)
            {
                throw new KeyNotFoundException($"No se encontro ningun registro con el id {request.Id}");
            }
            await _repositoryAsync.DeleteAsync(subcategory);
            await _repositoryAsync.SaveChangesAsync();
            return new Response<string>($"{subcategory.Name} eliminado correctamente", "Eliminado correctamente");
        }
    }
}
