using MediatR;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.ProjectAttachmentCategoryFeature.Commands.DeleteProjectAttachmentCategoryCommand
{
    public class DeleteProjectAttachmentCategoryCommand : IRequest<Response<string>>
    {
        public int Id { get; set; }
    }

    public class DeleteProjectAttachmentCategoryCommandHandler : IRequestHandler<DeleteProjectAttachmentCategoryCommand, Response<string>>
    {
        private readonly IRepositoryAsync<ProjectAttachmentCategory> _repositoryAsync;

        public DeleteProjectAttachmentCategoryCommandHandler(IRepositoryAsync<ProjectAttachmentCategory> repositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
        }

        public async Task<Response<string>> Handle(DeleteProjectAttachmentCategoryCommand request, CancellationToken cancellationToken)
        {
            var record = await _repositoryAsync.GetByIdAsync(request.Id);
            if (record == null)
            {
                throw new KeyNotFoundException($"No se encontro ningun registro con el id {request.Id}");
            }
            await _repositoryAsync.DeleteAsync(record);
            await _repositoryAsync.SaveChangesAsync();
            return new Response<string>($"Categoria eliminada correctamente", "Eliminado correctamente");
        }
    }
}
