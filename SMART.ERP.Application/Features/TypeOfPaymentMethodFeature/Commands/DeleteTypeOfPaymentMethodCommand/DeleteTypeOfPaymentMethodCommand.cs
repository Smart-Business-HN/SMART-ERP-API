using MediatR;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Features.BankFeature.Commands.DeleteBankCommand;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.ERP.Application.Features.TypeOfPaymentMethodFeature.Commands.DeleteTypeOfPaymentMethodCommand
{
    public class DeleteTypeOfPaymentMethodCommand : IRequest<Response<string>>
    {
        public int Id { get; set; }
    }

    public class DeleteTypeOfPaymentMethodCommandHandler : IRequestHandler<DeleteTypeOfPaymentMethodCommand, Response<string>>
    {
        private readonly IRepositoryAsync<TypeOfPaymentMethod> _repositoryAsync;

        public DeleteTypeOfPaymentMethodCommandHandler(IRepositoryAsync<TypeOfPaymentMethod> repositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
        }

        public async Task<Response<string>> Handle(DeleteTypeOfPaymentMethodCommand request, CancellationToken cancellationToken)
        {
            var checkBank = await _repositoryAsync.GetByIdAsync(request.Id);
            if (checkBank == null)
            {
                throw new KeyNotFoundException($"No se encontro un Tipo de Pago con id {request.Id}");
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
