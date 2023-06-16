using MediatR;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.DataSheetFeature.Commands.DeleteDataSheetCommand
{
    public class DeleteDataSheetCommand : IRequest<Response<string>>
    {
        public int Id { get; set; }
    }

    public class DeleteDataSheetCommandHandler : IRequestHandler<DeleteDataSheetCommand, Response<string>>
    {
        private readonly IRepositoryAsync<DataSheet> _repositoryAsync;

        public DeleteDataSheetCommandHandler(IRepositoryAsync<DataSheet> repositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
        }
        public async Task<Response<string>> Handle(DeleteDataSheetCommand request, CancellationToken cancellationToken)
        {
            var dataSheet = await _repositoryAsync.GetByIdAsync(request.Id);
            if (dataSheet == null)
            {
                throw new KeyNotFoundException($"No se encontro ningun registro con el id {request.Id}");
            }
            await _repositoryAsync.DeleteAsync(dataSheet);
            await _repositoryAsync.SaveChangesAsync();
            return new Response<string>($"{dataSheet.Name} eliminado correctamente", "Eliminado correctamente");
        }
    }
}
