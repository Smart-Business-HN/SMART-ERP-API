using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.NonBillableExpensePayment;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Domain.Enums;

namespace SMART.ERP.Application.Features.NonBillableExpensePaymentFeature.Commands.UpdateNonBillableExpensePaymentCommand
{
    public class UpdateNonBillableExpensePaymentCommand : IRequest<Response<NonBillableExpensePaymentDto>>
    {
        public int Id { get; set; }
        public int NonBillableExpenseId { get; set; }
        public int TypeOfPaymentMethodId { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public int? InternalBankAccountId { get; set; }
        public string? Attachment { get; set; }
    }

    public class UpdateNonBillableExpensePaymentCommandHandler : IRequestHandler<UpdateNonBillableExpensePaymentCommand, Response<NonBillableExpensePaymentDto>>
    {
        private readonly IRepositoryAsync<NonBillableExpensePayment> _repositoryAsync;
        private readonly IRepositoryAsync<NonBillableExpense> _nonBillableExpenseRepositoryAsync;
        private readonly IRepositoryAsync<InternalBankAccount> _internalBankAccountRepositoryAsync;
        private readonly IRepositoryAsync<TypeOfPaymentMethod> _typeOfPaymentMethodRepositoryAsync;
        private readonly IMapper _mapper;

        public UpdateNonBillableExpensePaymentCommandHandler(IRepositoryAsync<NonBillableExpensePayment> repositoryAsync, IRepositoryAsync<NonBillableExpense> invoiceRepositoryAsync, IRepositoryAsync<InternalBankAccount> internalBankAccountRepositoryAsync, IRepositoryAsync<TypeOfPaymentMethod> typeOfPaymentMethodRepositoryAsync, IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _nonBillableExpenseRepositoryAsync = invoiceRepositoryAsync;
            _internalBankAccountRepositoryAsync = internalBankAccountRepositoryAsync;
            _typeOfPaymentMethodRepositoryAsync = typeOfPaymentMethodRepositoryAsync;
            _mapper = mapper;
        }

        public async Task<Response<NonBillableExpensePaymentDto>> Handle(UpdateNonBillableExpensePaymentCommand request, CancellationToken cancellationToken)
        {
            var checkBillPaymen = await _repositoryAsync.GetByIdAsync(request.Id);
            if (checkBillPaymen == null)
            {
                throw new KeyNotFoundException($"No se encontro ningun pago el con id {request.Id}");
            }
            var checkNonBillableExpense = await _nonBillableExpenseRepositoryAsync.GetByIdAsync(request.NonBillableExpenseId);
            if (checkNonBillableExpense == null)
            {
                throw new KeyNotFoundException($"No se encontro un gasto con id {request.NonBillableExpenseId}");
            }
            if (checkNonBillableExpense.StatusId == 30)
            {
                throw new ApiException("Este gasto ya se encuentra pagada. Por lo cual no puede modificar este pago.");
            }
            var checkTypeOfPaymentMethod = await _typeOfPaymentMethodRepositoryAsync.GetByIdAsync(request.TypeOfPaymentMethodId);
            if (checkTypeOfPaymentMethod == null)
            {
                throw new KeyNotFoundException($"No se encontro una forma de pago con id {request.TypeOfPaymentMethodId}");
            }
            InternalBankAccount? newAccount = null;
            if (request.InternalBankAccountId != null)
            {
                newAccount = await _internalBankAccountRepositoryAsync.GetByIdAsync((int)request.InternalBankAccountId);
                if (newAccount == null)
                {
                    throw new KeyNotFoundException($"No se encontro una la cuenta bancaria con id {request.InternalBankAccountId}");
                }
            }
            decimal invoiceWithoutThisPayment = checkNonBillableExpense.Outstanding + checkBillPaymen.Amount;
            if (invoiceWithoutThisPayment < request.Amount)
            {
                throw new ApiException("Esta intentando pagar mas de lo debido en el gasto de origen.");
            }
            checkNonBillableExpense.Outstanding = invoiceWithoutThisPayment - request.Amount;
            if (invoiceWithoutThisPayment - request.Amount == 0)
            {
                checkNonBillableExpense.StatusId = 30;
            }

            decimal previousAmount = checkBillPaymen.Amount;
            int? previousAccountId = checkBillPaymen.InternalBankAccountId;

            checkBillPaymen.Amount = request.Amount;
            checkBillPaymen.NonBillableExpenseId = request.NonBillableExpenseId;
            checkBillPaymen.TypeOfPaymentMethodId = request.TypeOfPaymentMethodId;
            checkBillPaymen.Date = request.Date;
            checkBillPaymen.Attachment = request.Attachment;
            checkBillPaymen.InternalBankAccountId = request.InternalBankAccountId;

            await _repositoryAsync.UpdateAsync(checkBillPaymen);
            await _repositoryAsync.SaveChangesAsync();
            await _nonBillableExpenseRepositoryAsync.UpdateAsync(checkNonBillableExpense);
            await _nonBillableExpenseRepositoryAsync.SaveChangesAsync();

            if (previousAccountId != null)
            {
                var oldAccount = await _internalBankAccountRepositoryAsync.GetByIdAsync((int)previousAccountId);
                if (oldAccount != null && oldAccount.AccountType == InternalBankAccountType.CreditCard)
                {
                    oldAccount.CurrentAmount -= previousAmount;
                    await _internalBankAccountRepositoryAsync.UpdateAsync(oldAccount);
                    await _internalBankAccountRepositoryAsync.SaveChangesAsync();
                }
            }
            if (newAccount != null && newAccount.AccountType == InternalBankAccountType.CreditCard)
            {
                newAccount.CurrentAmount += request.Amount;
                await _internalBankAccountRepositoryAsync.UpdateAsync(newAccount);
                await _internalBankAccountRepositoryAsync.SaveChangesAsync();
            }

            var dto = _mapper.Map<NonBillableExpensePaymentDto>(checkBillPaymen);
            return new Response<NonBillableExpensePaymentDto>(dto, $"Pago actualizado correctamente");
        }
    }
}
