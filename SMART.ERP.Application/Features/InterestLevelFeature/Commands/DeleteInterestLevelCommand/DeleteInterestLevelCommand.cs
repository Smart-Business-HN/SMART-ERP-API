using MediatR;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.InterestLevelFeature.Commands.DeleteInterestLevelCommand
{
    public class DeleteInterestLevelCommand : IRequest<Response<string>>
    {
        public int Id { get; set; }
    }

    public class DeleteInterestLevelCommandHandler : IRequestHandler<DeleteInterestLevelCommand, Response<string>>
    {
        private readonly IRepositoryAsync<InterestLevel> _repositoryAsync;

        public DeleteInterestLevelCommandHandler(IRepositoryAsync<InterestLevel> repositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
        }
        public async Task<Response<string>> Handle(DeleteInterestLevelCommand request, CancellationToken cancellationToken)
        {
            var interestLevel = await _repositoryAsync.GetByIdAsync(request.Id);
            if (interestLevel == null)
            {
                throw new KeyNotFoundException($"No se encontro ningun registro con el id {request.Id}");
            }
            await _repositoryAsync.DeleteAsync(interestLevel);
            await _repositoryAsync.SaveChangesAsync();
            return new Response<string>($"{interestLevel.Name} eliminado correctamente", "Eliminado correctamente");
        }
    }
}
