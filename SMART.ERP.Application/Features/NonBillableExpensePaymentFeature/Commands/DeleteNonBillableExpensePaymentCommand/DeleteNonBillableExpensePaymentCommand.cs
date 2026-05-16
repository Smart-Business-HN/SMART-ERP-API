using MediatR;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Domain.Enums;

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
        private readonly IRepositoryAsync<InternalBankAccount> _internalBankAccountRepositoryAsync;

        public DeleteNonBillableExpensePaymentCommandHandler(IRepositoryAsync<NonBillableExpensePayment> repositoryAsync, IRepositoryAsync<NonBillableExpense> nonBillableExpenseRepositoryAsync, IRepositoryAsync<InternalBankAccount> internalBankAccountRepositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
            _nonBillableExpenseRepositoryAsync = nonBillableExpenseRepositoryAsync;
            _internalBankAccountRepositoryAsync = internalBankAccountRepositoryAsync;
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

            InternalBankAccount? account = null;
            if (checkNonBillableExpensePayment.InternalBankAccountId != null)
            {
                account = await _internalBankAccountRepositoryAsync.GetByIdAsync((int)checkNonBillableExpensePayment.InternalBankAccountId);
            }
            decimal amountToReverse = checkNonBillableExpensePayment.Amount;

            try
            {

                await _repositoryAsync.DeleteAsync(checkNonBillableExpensePayment);
                await _repositoryAsync.SaveChangesAsync();
                await _nonBillableExpenseRepositoryAsync.UpdateAsync(invoice);
                await _nonBillableExpenseRepositoryAsync.SaveChangesAsync();

                if (account != null && account.AccountType == InternalBankAccountType.CreditCard)
                {
                    account.CurrentAmount -= amountToReverse;
                    await _internalBankAccountRepositoryAsync.UpdateAsync(account);
                    await _internalBankAccountRepositoryAsync.SaveChangesAsync();
                }

                return new Response<string>("Eliminado correctamente");
            }
            catch (Exception)
            {
                throw new ApiException("Ocurrio un error al tratar de eliminar este registro, verifica que no se este utilizando en otro registro.");
            }
        }
    }
}
