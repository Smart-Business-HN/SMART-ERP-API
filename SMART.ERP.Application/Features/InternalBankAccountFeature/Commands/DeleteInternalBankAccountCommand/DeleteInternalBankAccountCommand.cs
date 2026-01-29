using MediatR;
using Microsoft.AspNetCore.OutputCaching;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.InternalBankAccountFeature.Commands.DeleteInternalBankAccountCommand
{
    public class DeleteInternalBankAccountCommand : IRequest<Response<string>>
    {
        public int Id { get; set; }
    }

    public class DeleteInternalBankAccountCommandHandler : IRequestHandler<DeleteInternalBankAccountCommand, Response<string>>
    {
        private readonly IRepositoryAsync<InternalBankAccount> _repositoryAsync;
        private readonly IOutputCacheStore _outputCacheStored;

        public DeleteInternalBankAccountCommandHandler(IRepositoryAsync<InternalBankAccount> repositoryAsync, IOutputCacheStore outputCacheStored)
        {
            _repositoryAsync = repositoryAsync;
            _outputCacheStored = outputCacheStored;
        }

        public async Task<Response<string>> Handle(DeleteInternalBankAccountCommand request, CancellationToken cancellationToken)
        {
            var checkInternalBankAccount = await _repositoryAsync.GetByIdAsync(request.Id);
            if (checkInternalBankAccount == null)
            {
                throw new KeyNotFoundException($"No se encontro una cuenta bancaria con el id {request.Id}");
            }

            try
            {
                await _repositoryAsync.DeleteAsync(checkInternalBankAccount);
                await _repositoryAsync.SaveChangesAsync();
                await _outputCacheStored.EvictByTagAsync("cache_internalBankAccounts", cancellationToken);
                return new Response<string>("Eliminado correctamente");
            }
            catch (Exception)
            {
                throw new ApiException("Ocurrio un error al tratar de eliminar este registro, verifica que no se este utilizando en otro registro.");
            }
        }
    }
}
