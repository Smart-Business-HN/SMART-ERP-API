using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.PurchaseBillPayment;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.JwtService;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Domain.Enums;

namespace SMART.ERP.Application.Features.PurchaseBillPaymentFeature.Commands.CreatePurchaseBillPaymentCommand
{
    public class CreatePurchaseBillPaymentCommand : IRequest<Response<PurchaseBillPaymentDto>>
    {
        public int PurchaseBillId { get; set; }
        public int TypeOfPaymentMethodId { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public int? InternalBankAccountId { get; set; }
        public string? Attachment { get; set; }
    }
    public class CreatePurchaseBillPaymentCommandHandler : IRequestHandler<CreatePurchaseBillPaymentCommand, Response<PurchaseBillPaymentDto>>
    {
        private readonly IRepositoryAsync<PurchaseBillPayment> _repositoryAsync;
        private readonly IRepositoryAsync<PurchaseBill> _purchaseBillRepositoryAsync;
        private readonly IRepositoryAsync<InternalBankAccount> _internalBankAccountRepositoryAsync;
        private readonly IRepositoryAsync<TypeOfPaymentMethod> _typeOfPaymentMethodRepositoryAsync;
        private readonly IJwtService _jwtService;
        private readonly IMapper _mapper;
        public CreatePurchaseBillPaymentCommandHandler(IRepositoryAsync<PurchaseBillPayment> repositoryAsync, IRepositoryAsync<PurchaseBill> purchaseBillRepositoryAsync, IRepositoryAsync<InternalBankAccount> internalBankAccountRepositoryAsync, IRepositoryAsync<TypeOfPaymentMethod> typeOfPaymentMethodRepositoryAsync, IJwtService jwtService, IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _purchaseBillRepositoryAsync = purchaseBillRepositoryAsync;
            _internalBankAccountRepositoryAsync = internalBankAccountRepositoryAsync;
            _typeOfPaymentMethodRepositoryAsync = typeOfPaymentMethodRepositoryAsync;
            _jwtService = jwtService;
            _mapper = mapper;
        }
        public async Task<Response<PurchaseBillPaymentDto>> Handle(CreatePurchaseBillPaymentCommand request, CancellationToken cancellationToken)
        {
            var checkPurchaseBill = await _purchaseBillRepositoryAsync.GetByIdAsync(request.PurchaseBillId);
            if (checkPurchaseBill == null)
            {
                throw new KeyNotFoundException($"No se encontro una factura con id {request.PurchaseBillId}");
            }
            if (checkPurchaseBill.StatusId == 17)
            {
                throw new ApiException("Esta factura ya se encuentra pagada.");
            }
            if (checkPurchaseBill.Outstanding < request.Amount)
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
            checkPurchaseBill.Outstanding = checkPurchaseBill.Outstanding - request.Amount;
            if (checkPurchaseBill.Outstanding == 0)
            {
                checkPurchaseBill.StatusId = 28;
            }
            var newRecord = _mapper.Map<PurchaseBillPayment>(request);
            newRecord.CreationDate = DateTime.Now;
            newRecord.CreatedBy = _jwtService.GetSubjectToken();
            var response = await _repositoryAsync.AddAsync(newRecord);
            await _repositoryAsync.SaveChangesAsync();
            await _purchaseBillRepositoryAsync.UpdateAsync(checkPurchaseBill);
            await _purchaseBillRepositoryAsync.SaveChangesAsync();

            if (checkInternalBankAccount != null && checkInternalBankAccount.AccountType == InternalBankAccountType.CreditCard)
            {
                checkInternalBankAccount.CurrentAmount += request.Amount;
                await _internalBankAccountRepositoryAsync.UpdateAsync(checkInternalBankAccount);
                await _internalBankAccountRepositoryAsync.SaveChangesAsync();
            }

            var dto = _mapper.Map<PurchaseBillPaymentDto>(response);
            return new Response<PurchaseBillPaymentDto>(dto, $"Pago realizado exitosamente");
        }
    }
}
