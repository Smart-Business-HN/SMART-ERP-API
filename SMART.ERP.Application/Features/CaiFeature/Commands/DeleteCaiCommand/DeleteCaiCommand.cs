using MediatR;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.CaiFeature.Commands.DeleteCaiCommand
{
    public class DeleteCaiCommand : IRequest<Response<string>>
    {
        public int Id { get; set; }
    }
    public class DeleteCaiCommandHandler : IRequestHandler<DeleteCaiCommand, Response<string>>
    {
        private readonly IRepositoryAsync<Cai> _repositoryAsync;
        public DeleteCaiCommandHandler(IRepositoryAsync<Cai> repoAsync) { _repositoryAsync = repoAsync; }
        public async Task<Response<string>> Handle(DeleteCaiCommand request, CancellationToken cancellationToken)
        {
            var cai = await _repositoryAsync.GetByIdAsync(request.Id);
            if(cai == null)
            {
                throw new KeyNotFoundException($"No se encontro ningun registro con el id {request.Id}");
            }
            await _repositoryAsync.DeleteAsync(cai);
            await _repositoryAsync.SaveChangesAsync();
            return new Response<string>($"{cai.Name} eliminado correctamente", "Eliminado Correctamente");
        }
    }
}
