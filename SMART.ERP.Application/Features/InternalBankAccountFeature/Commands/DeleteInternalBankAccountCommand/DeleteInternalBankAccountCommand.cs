using MediatR;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Features.CityFeature.Commands.DeleteCityCommand;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.ERP.Application.Features.InternalBankAccountFeature.Commands.DeleteInternalBankAccountCommand
{
    public class DeleteInternalBankAccountCommand : IRequest<Response<string>>
    {
        public int Id { get; set; }
    }

    public class DeleteInternalBankAccountCommandHandler : IRequestHandler<DeleteInternalBankAccountCommand, Response<string>>
    {
        private readonly IRepositoryAsync<InternalBankAccount> _repositoryAsync;

        public DeleteInternalBankAccountCommandHandler(IRepositoryAsync<InternalBankAccount> repositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
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

                return new Response<string>("Eliminado correctamente");
            }
            catch (Exception)
            {
                throw new ApiException("Ocurrio un error al tratar de eliminar este registro, verifica que no se este utilizando en otro registro.");
            }
        }
    }
}
