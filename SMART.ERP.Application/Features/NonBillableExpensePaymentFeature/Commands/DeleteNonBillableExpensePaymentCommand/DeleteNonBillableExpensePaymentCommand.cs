using MediatR;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.NonBillableExpensePaymentFeature.Commands.DeleteNonBillableExpensePaymentCommand
{
    public class DeleteNonBillableExpensePaymentCommand : IRequest<Response<string>>
    {
        public int Id { get; set; }
    }

    public class DeleteNonBillableExpensePaymentCommandHandler : IRequestHandler<DeleteNonBillableExpensePaymentCommand, Response<string>>
    {
        private readonly IRepositoryAsync<NonBillableExpensePayment> _repositoryAsync;
        private readonly IRepositoryAsync<NonBillableExpense> _nonBillableExpenseRepositoryAsync;

        public DeleteNonBillableExpensePaymentCommandHandler(IRepositoryAsync<NonBillableExpensePayment> repositoryAsync, IRepositoryAsync<NonBillableExpense> nonBillableExpenseRepositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
            _nonBillableExpenseRepositoryAsync = nonBillableExpenseRepositoryAsync;
        }

        public async Task<Response<string>> Handle(DeleteNonBillableExpensePaymentCommand request, CancellationToken cancellationToken)
        {
            var checkNonBillableExpensePayment = await _repositoryAsync.GetByIdAsync(request.Id);
            if (checkNonBillableExpensePayment == null)
            {
                throw new KeyNotFoundException($"No se encontro un pago con id {request.Id}");
            }
            var invoice = await _nonBillableExpenseRepositoryAsync.GetByIdAsync(checkNonBillableExpensePayment.NonBillableExpenseId);
            invoice!.Outstanding = invoice.Outstanding + checkNonBillableExpensePayment.Amount;

            try
            {

                await _repositoryAsync.DeleteAsync(checkNonBillableExpensePayment);
                await _repositoryAsync.SaveChangesAsync();
                await _nonBillableExpenseRepositoryAsync.UpdateAsync(invoice);
                await _nonBillableExpenseRepositoryAsync.SaveChangesAsync();

                return new Response<string>("Eliminado correctamente");
            }
            catch (Exception)
            {
                throw new ApiException("Ocurrio un error al tratar de eliminar este registro, verifica que no se este utilizando en otro registro.");
            }
        }
    }
}
