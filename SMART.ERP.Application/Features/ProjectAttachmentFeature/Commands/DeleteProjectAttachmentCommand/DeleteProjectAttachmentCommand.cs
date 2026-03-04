using MediatR;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.BlobStorageService;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.ProjectAttachmentFeature.Commands.DeleteProjectAttachmentCommand
{
    public class DeleteProjectAttachmentCommand : IRequest<Response<string>>
    {
        public int Id { get; set; }
    }

    public class DeleteProjectAttachmentCommandHandler : IRequestHandler<DeleteProjectAttachmentCommand, Response<string>>
    {
        private readonly IRepositoryAsync<ProjectAttachment> _repositoryAsync;
        private readonly IBlobStorageService _blobStorageService;

        public DeleteProjectAttachmentCommandHandler(
            IRepositoryAsync<ProjectAttachment> repositoryAsync,
            IBlobStorageService blobStorageService)
        {
            _repositoryAsync = repositoryAsync;
            _blobStorageService = blobStorageService;
        }

        public async Task<Response<string>> Handle(DeleteProjectAttachmentCommand request, CancellationToken cancellationToken)
        {
            var projectAttachment = await _repositoryAsync.GetByIdAsync(request.Id);
            if (projectAttachment == null)
            {
                throw new KeyNotFoundException($"No se encontro ningun registro con el id {request.Id}");
            }

            // Extraer el nombre del archivo de la URL para eliminar el blob de Azure
            var fileName = Path.GetFileName(new Uri(projectAttachment.Url).LocalPath);
            await _blobStorageService.DeleteFileAsync(fileName);

            await _repositoryAsync.DeleteAsync(projectAttachment);
            await _repositoryAsync.SaveChangesAsync();
            return new Response<string>($"Adjunto eliminado correctamente", "Eliminado correctamente");
        }
    }
}
