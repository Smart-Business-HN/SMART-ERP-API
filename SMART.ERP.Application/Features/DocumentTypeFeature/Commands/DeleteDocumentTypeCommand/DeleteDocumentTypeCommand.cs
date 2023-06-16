using MediatR;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.DocumentTypeFeature.Commands.DeleteDocumentTypeCommand
{
    public class DeleteDocumentTypeCommand : IRequest<Response<string>>
    {
        public int Id { get; set; }
    }

    public class DeleteDocumentTypeCommandHandler : IRequestHandler<DeleteDocumentTypeCommand, Response<string>>
    {
        private readonly IRepositoryAsync<DocumentType> _repositoryAsync;

        public DeleteDocumentTypeCommandHandler(IRepositoryAsync<DocumentType> repositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
        }

        public async Task<Response<string>> Handle(DeleteDocumentTypeCommand request, CancellationToken cancellationToken)
        {
            var documentType = await _repositoryAsync.GetByIdAsync(request.Id);
            if (documentType == null)
            {
                throw new KeyNotFoundException($"No se encontro ningun registro con el id {request.Id}");
            }
            await _repositoryAsync.DeleteAsync(documentType);
            await _repositoryAsync.SaveChangesAsync();
            return new Response<string>($"{documentType.Name} eliminado correctamente", "Eliminado correctamente");
        }
    }
}
