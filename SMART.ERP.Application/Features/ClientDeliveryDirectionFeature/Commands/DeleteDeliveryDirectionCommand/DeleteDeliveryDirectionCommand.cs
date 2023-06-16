using MediatR;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.MASTER.Domain.Entities;

namespace SMART.ERP.Application.Features.ClientDeliveryDirectionFeature.Commands.DeleteDeliveryDirectionCommand
{
    public class DeleteDeliveryDirectionCommand : IRequest<Response<string>>
    {
        public int Id { get; set; }
    }

    public class DeleteDeliveryDirectionCommandHandler : IRequestHandler<DeleteDeliveryDirectionCommand, Response<string>>
    {
        private readonly IRepositoryHNAsync<DeliveryDirection> _repositoryHNAsync;

        public DeleteDeliveryDirectionCommandHandler(IRepositoryHNAsync<DeliveryDirection> repositoryHNAsync)
        {
            _repositoryHNAsync = repositoryHNAsync;
        }
        public async Task<Response<string>> Handle(DeleteDeliveryDirectionCommand request, CancellationToken cancellationToken)
        {
            var checkIfExist = await _repositoryHNAsync.GetByIdAsync(request.Id);
            if (checkIfExist == null)
            {
                throw new KeyNotFoundException($"No se encontro la direccion de envio con id {request.Id}");
            }
            await _repositoryHNAsync.DeleteAsync(checkIfExist);
            await _repositoryHNAsync.SaveChangesAsync();
            return new Response<string>("Eliminado Correctamente");
        }
    }
}
