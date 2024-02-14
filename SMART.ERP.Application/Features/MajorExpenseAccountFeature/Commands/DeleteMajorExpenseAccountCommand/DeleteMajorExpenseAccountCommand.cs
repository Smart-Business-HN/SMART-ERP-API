using MediatR;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.MajorExpenseAccountFeature.Commands.DeleteMajorExpenseAccountCommand
{
    public class DeleteMajorExpenseAccountCommand : IRequest<Response<string>>
    {
        public int Id { get; set; }
    }
    public class DeleteMajorExpenseAccountCommandHandler : IRequestHandler<DeleteMajorExpenseAccountCommand,Response<string>>
    {
        private readonly IRepositoryAsync<MajorExpenseAccount> _repositoryAsync;
        public DeleteMajorExpenseAccountCommandHandler(IRepositoryAsync<MajorExpenseAccount> repositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
        }
        public async Task<Response<string>> Handle(DeleteMajorExpenseAccountCommand request, CancellationToken cancellationToken)
        {
            var checkAccount = await _repositoryAsync.GetByIdAsync(request.Id);
            if (checkAccount == null)
            {
                throw new KeyNotFoundException($"No se encontro una cuenta con id {request.Id}");
            }

            try
            {
                await _repositoryAsync.DeleteAsync(checkAccount);
                await _repositoryAsync.SaveChangesAsync();

                return new Response<string>("Eliminado correctamente");
            }
            catch (Exception)
            {
                throw new ApiException("Ocurrio un error al tratar de eliminar este registro, verifica que no se este utilizando en otro registro.");
            }
        }
    }
}
