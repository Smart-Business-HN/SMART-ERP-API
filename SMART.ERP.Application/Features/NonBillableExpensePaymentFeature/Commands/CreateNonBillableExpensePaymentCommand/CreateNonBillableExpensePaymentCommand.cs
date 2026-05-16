using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.OutputCaching;
using SMART.ERP.Application.DTOs.NonBillableExpensePayment;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.JwtService;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Domain.Enums;

namespace SMART.ERP.Application.Features.NonBillableExpensePaymentFeature.Commands.CreateNonBillableExpensePaymentCommand
{
    public class CreateNonBillableExpensePaymentCommand : IRequest<Response<NonBillableExpensePaymentDto>>
    {
        public int NonBillableExpenseId { get; set; }
        public int TypeOfPaymentMethodId { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public int? InternalBankAccountId { get; set; }
        public string? Attachment { get; set; }
    }

    public class CreateNonBillableExpensePaymentCommandHandler : IRequestHandler<CreateNonBillableExpensePaymentCommand, Response<NonBillableExpensePaymentDto>>
    {
        private readonly IRepositoryAsync<NonBillableExpensePayment> _repositoryAsync;
        private readonly IRepositoryAsync<NonBillableExpense> _nonBillableExpenseRepositoryAsync;
        private readonly IRepositoryAsync<InternalBankAccount> _internalBankAccountRepositoryAsync;
        private readonly IRepositoryAsync<TypeOfPaymentMethod> _typeOfPaymentMethodRepositoryAsync;
        private readonly IJwtService _jwtService;
        private readonly IMapper _mapper;
        private readonly IOutputCacheStore _outputCacheStored;
        public CreateNonBillableExpensePaymentCommandHandler(IRepositoryAsync<NonBillableExpensePayment> repositoryAsync, IJwtService jwtService, IRepositoryAsync<NonBillableExpense> nonBillableExpenseRepositoryAsync, IRepositoryAsync<InternalBankAccount> internalBankAccountRepositoryAsync, IRepositoryAsync<TypeOfPaymentMethod> typeOfPaymentMethodRepositoryAsync, IMapper mapper, IOutputCacheStore outputCacheStored)
        {
            _repositoryAsync = repositoryAsync;
            _nonBillableExpenseRepositoryAsync = nonBillableExpenseRepositoryAsync;
            _internalBankAccountRepositoryAsync = internalBankAccountRepositoryAsync;
            _typeOfPaymentMethodRepositoryAsync = typeOfPaymentMethodRepositoryAsync;
            _mapper = mapper;
            _outputCacheStored = outputCacheStored;
            _jwtService = jwtService;
        }

        public async Task<Response<NonBillableExpensePaymentDto>> Handle(CreateNonBillableExpensePaymentCommand request, CancellationToken cancellationToken)
        {
            var nonBillableExpense = await _nonBillableExpenseRepositoryAsync.GetByIdAsync(request.NonBillableExpenseId);
            if (nonBillableExpense == null)
            {
                throw new KeyNotFoundException($"No se encontro un gasto con id {request.NonBillableExpenseId}");
            }
            if (nonBillableExpense.StatusId == 30)
            {
                throw new ApiException("Este gasto ya se encuentra pagada.");
            }
            if (nonBillableExpense.Outstanding < request.Amount)
            {
                throw new ApiException("No se permiten pagos mayores al saldo adeudado.");
            }
            var checkTypeOfPaymentMethod = await _typeOfPaymentMethodRepositoryAsync.GetByIdAsync(request.TypeOfPaymentMethodId);
            if (checkTypeOfPaymentMethod == null)
            {
                throw new KeyNotFoundException($"No se encontro una forma de pago con id {request.TypeOfPaymentMethodId}");
            }
            InternalBankAccount? checkInternalBankAccount = null;
            if (request.InternalBankAccountId != null)
            {
                checkInternalBankAccount = await _internalBankAccountRepositoryAsync.GetByIdAsync((int)request.InternalBankAccountId);
                if (checkInternalBankAccount == null)
                {
                    throw new KeyNotFoundException($"No se encontro una la cuenta bancaria con id {request.InternalBankAccountId}");
                }
            }
            nonBillableExpense.Outstanding = nonBillableExpense.Outstanding - request.Amount;
            if (nonBillableExpense.Outstanding == 0)
            {
                nonBillableExpense.StatusId = 30;
            }

            var newRecord = _mapper.Map<NonBillableExpensePayment>(request);
            newRecord.CreationDate = DateTime.Now;
            newRecord.CreatedBy = _jwtService.GetSubjectToken();
            var response = await _repositoryAsync.AddAsync(newRecord);
            await _repositoryAsync.SaveChangesAsync();
            await _nonBillableExpenseRepositoryAsync.UpdateAsync(nonBillableExpense);
            await _nonBillableExpenseRepositoryAsync.SaveChangesAsync();

            if (checkInternalBankAccount != null && checkInternalBankAccount.AccountType == InternalBankAccountType.CreditCard)
            {
                checkInternalBankAccount.CurrentAmount += request.Amount;
                await _internalBankAccountRepositoryAsync.UpdateAsync(checkInternalBankAccount);
                await _internalBankAccountRepositoryAsync.SaveChangesAsync();
            }

            var dto = _mapper.Map<NonBillableExpensePaymentDto>(response);
            await _outputCacheStored.EvictByTagAsync("cache_nonBillableExpense", cancellationToken);
            return new Response<NonBillableExpensePaymentDto>(dto, $"Pago recibido exitosamente");
        }
    }
}
