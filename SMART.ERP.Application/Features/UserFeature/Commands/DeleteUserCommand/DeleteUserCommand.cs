using MediatR;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.UserFeature.Commands.DeleteUserCommand
{
    public class DeleteUserCommand : IRequest<Response<string>>
    {
        public Guid Id { get; set; }
    }

    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, Response<string>>
    {
        private readonly IRepositoryAsync<User> _repositoryAsync;

        public DeleteUserCommandHandler(IRepositoryAsync<User> repositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
        }
        public async Task<Response<string>> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _repositoryAsync.GetByIdAsync(request.Id);
            if (user == null)
            {
                throw new KeyNotFoundException($"No se encontro ningun registro con el id {request.Id}");
            }
            await _repositoryAsync.DeleteAsync(user);
            await _repositoryAsync.SaveChangesAsync();
            return new Response<string>($"{user.FullName} eliminado correctamente", "Eliminado correctamente");
        }
    }
}
