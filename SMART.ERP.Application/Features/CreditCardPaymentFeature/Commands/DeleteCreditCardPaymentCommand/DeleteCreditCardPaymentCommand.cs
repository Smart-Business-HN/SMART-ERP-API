using MediatR;
using Microsoft.AspNetCore.OutputCaching;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.AccountingPostingService;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.CreditCardPaymentFeature.Commands.DeleteCreditCardPaymentCommand
{
    /// <summary>
    /// Elimina un pago de TC y reversa el asiento contable asociado. Restaura los saldos
    /// (CurrentAmount) tanto en la TC como en el banco origen.
    /// </summary>
    public class DeleteCreditCardPaymentCommand : IRequest<Response<string>>
    {
        public int Id { get; set; }
    }

    public class DeleteCreditCardPaymentCommandHandler : IRequestHandler<DeleteCreditCardPaymentCommand, Response<string>>
    {
        private readonly IRepositoryAsync<CreditCardPayment> _repositoryAsync;
        private readonly IRepositoryAsync<InternalBankAccount> _bankRepository;
        private readonly IOutputCacheStore _outputCacheStored;
        private readonly IAccountingPostingService _accountingPostingService;

        public DeleteCreditCardPaymentCommandHandler(
            IRepositoryAsync<CreditCardPayment> repositoryAsync,
            IRepositoryAsync<InternalBankAccount> bankRepository,
            IOutputCacheStore outputCacheStored,
            IAccountingPostingService accountingPostingService)
        {
            _repositoryAsync = repositoryAsync;
            _bankRepository = bankRepository;
            _outputCacheStored = outputCacheStored;
            _accountingPostingService = accountingPostingService;
        }

        public async Task<Response<string>> Handle(DeleteCreditCardPaymentCommand request, CancellationToken cancellationToken)
        {
            var payment = await _repositoryAsync.GetByIdAsync(request.Id, cancellationToken)
                ?? throw new ApiException($"No existe el pago de TC con Id {request.Id}.");

            // Reversa contable primero — si falla, no tocamos los saldos.
            await _accountingPostingService.ReverseDocumentPostingAsync("CreditCardPayment", payment.Id, cancellationToken);

            // Restaurar saldos en TC y banco origen.
            var creditCard = await _bankRepository.GetByIdAsync(payment.CreditCardInternalBankAccountId, cancellationToken);
            if (creditCard != null)
            {
                creditCard.CurrentAmount += payment.Amount;
                await _bankRepository.UpdateAsync(creditCard, cancellationToken);
            }
            var source = await _bankRepository.GetByIdAsync(payment.SourceInternalBankAccountId, cancellationToken);
            if (source != null)
            {
                source.CurrentAmount += payment.Amount;
                await _bankRepository.UpdateAsync(source, cancellationToken);
            }
            await _bankRepository.SaveChangesAsync(cancellationToken);

            await _repositoryAsync.DeleteAsync(payment, cancellationToken);
            await _repositoryAsync.SaveChangesAsync(cancellationToken);
            await _outputCacheStored.EvictByTagAsync("cache_creditCardPayment", cancellationToken);

            return new Response<string>($"Pago de TC {payment.Code} eliminado y asiento reversado.");
        }
    }
}
