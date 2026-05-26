using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.OutputCaching;
using SMART.ERP.Application.DTOs.CreditCardPayment;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.AccountingPostingService;
using SMART.ERP.Application.Services.JwtService;
using SMART.ERP.Application.Specifications.CreditCardPaymentSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Domain.Enums;

namespace SMART.ERP.Application.Features.CreditCardPaymentFeature.Commands.CreateCreditCardPaymentCommand
{
    /// <summary>
    /// Registra un pago a una tarjeta de crédito desde una cuenta bancaria (Checking/Savings).
    /// Disminuye el saldo adeudado de la TC y disminuye el saldo del banco origen. Si el posteo
    /// automático está activo, genera Dr (LedgerAccount de la TC) / Cr (LedgerAccount del banco).
    /// </summary>
    public class CreateCreditCardPaymentCommand : IRequest<Response<CreditCardPaymentDto>>
    {
        public int CreditCardInternalBankAccountId { get; set; }
        public int SourceInternalBankAccountId { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public string? Reference { get; set; }
        public string? Notes { get; set; }
    }

    public class CreateCreditCardPaymentCommandHandler : IRequestHandler<CreateCreditCardPaymentCommand, Response<CreditCardPaymentDto>>
    {
        private readonly IRepositoryAsync<CreditCardPayment> _repositoryAsync;
        private readonly IRepositoryAsync<InternalBankAccount> _bankRepository;
        private readonly IMapper _mapper;
        private readonly IJwtService _jwtService;
        private readonly IOutputCacheStore _outputCacheStored;
        private readonly IAccountingPostingService _accountingPostingService;

        public CreateCreditCardPaymentCommandHandler(
            IRepositoryAsync<CreditCardPayment> repositoryAsync,
            IRepositoryAsync<InternalBankAccount> bankRepository,
            IMapper mapper,
            IJwtService jwtService,
            IOutputCacheStore outputCacheStored,
            IAccountingPostingService accountingPostingService)
        {
            _repositoryAsync = repositoryAsync;
            _bankRepository = bankRepository;
            _mapper = mapper;
            _jwtService = jwtService;
            _outputCacheStored = outputCacheStored;
            _accountingPostingService = accountingPostingService;
        }

        public async Task<Response<CreditCardPaymentDto>> Handle(CreateCreditCardPaymentCommand request, CancellationToken cancellationToken)
        {
            if (request.Amount <= 0)
                throw new ApiException("El monto del pago debe ser mayor a cero.");
            if (request.CreditCardInternalBankAccountId == request.SourceInternalBankAccountId)
                throw new ApiException("La cuenta de la TC y la cuenta de origen no pueden ser la misma.");

            var creditCard = await _bankRepository.GetByIdAsync(request.CreditCardInternalBankAccountId, cancellationToken)
                ?? throw new ApiException($"No existe la tarjeta de crédito con Id {request.CreditCardInternalBankAccountId}.");
            if (creditCard.AccountType != InternalBankAccountType.CreditCard)
                throw new ApiException($"La cuenta '{creditCard.Name}' no es una tarjeta de crédito.");

            var source = await _bankRepository.GetByIdAsync(request.SourceInternalBankAccountId, cancellationToken)
                ?? throw new ApiException($"No existe la cuenta bancaria origen con Id {request.SourceInternalBankAccountId}.");
            if (source.AccountType == InternalBankAccountType.CreditCard)
                throw new ApiException($"La cuenta origen '{source.Name}' no puede ser una tarjeta de crédito.");

            var nextCode = await GenerateNextCodeAsync(cancellationToken);
            var newRecord = _mapper.Map<CreditCardPayment>(request);
            newRecord.Code = nextCode;
            newRecord.CreationDate = DateTime.Now;
            newRecord.CreatedBy = _jwtService.GetSubjectToken();

            var created = await _repositoryAsync.AddAsync(newRecord, cancellationToken);
            await _repositoryAsync.SaveChangesAsync(cancellationToken);

            // Disminuye saldo adeudado de la TC (CurrentAmount en TC representa la deuda).
            creditCard.CurrentAmount -= request.Amount;
            await _bankRepository.UpdateAsync(creditCard, cancellationToken);
            // Disminuye saldo del banco origen (sale el dinero).
            source.CurrentAmount -= request.Amount;
            await _bankRepository.UpdateAsync(source, cancellationToken);
            await _bankRepository.SaveChangesAsync(cancellationToken);

            await _accountingPostingService.PostCreditCardPaymentAsync(created.Id, cancellationToken);
            await _outputCacheStored.EvictByTagAsync("cache_creditCardPayment", cancellationToken);

            var full = await _repositoryAsync.FirstOrDefaultAsync(new FilterCreditCardPaymentByIdSpecification(created.Id), cancellationToken);
            var dto = _mapper.Map<CreditCardPaymentDto>(full);
            return new Response<CreditCardPaymentDto>(dto, $"Pago de tarjeta de crédito {nextCode} registrado.");
        }

        private async Task<string> GenerateNextCodeAsync(CancellationToken ct)
        {
            var all = await _repositoryAsync.ListAsync(ct);
            var nextSeq = all.Count + 1;
            return $"CCP{nextSeq:00000000}";
        }
    }
}
