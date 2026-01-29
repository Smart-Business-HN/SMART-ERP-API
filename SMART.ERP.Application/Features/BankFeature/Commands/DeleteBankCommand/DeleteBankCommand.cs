using MediatR;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.BankFeature.Commands.DeleteBankCommand
{
    public class DeleteBankCommand : IRequest<Response<string>>
    {
        public int Id { get; set; }
    }

    public class DeleteBankCommandHandler : IRequestHandler<DeleteBankCommand, Response<string>>
    {
        private readonly IRepositoryAsync<Bank> _repositoryAsync;

        public DeleteBankCommandHandler(IRepositoryAsync<Bank> repositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
        }

        public async Task<Response<string>> Handle(DeleteBankCommand request, CancellationToken cancellationToken)
        {
            var checkBank = await _repositoryAsync.GetByIdAsync(request.Id);
            if (checkBank == null)
            {
                throw new KeyNotFoundException($"No se encontro un banco con id {request.Id}");
            }

            try
            {
                await _repositoryAsync.DeleteAsync(checkBank);
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
