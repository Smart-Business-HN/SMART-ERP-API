using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.OutputCaching;
using SMART.ERP.Application.DTOs.InternalBankAccount;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Domain.Enums;

namespace SMART.ERP.Application.Features.InternalBankAccountFeature.Commands.UpdateInternalBankAccountCommand
{
    public class UpdateInternalBankAccountCommand : IRequest<Response<InternalBankAccountDto>>
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int BankId { get; set; }
        public string? AccountNumber { get; set; }
        public decimal CurrentAmount { get; set; }
        public InternalBankAccountType AccountType { get; set; }
        public string? CardLastFour { get; set; }
    }

    public class UpdateInternalBankAccountCommandHandler : IRequestHandler<UpdateInternalBankAccountCommand, Response<InternalBankAccountDto>>
    {
        private readonly IRepositoryAsync<InternalBankAccount> _repositoryAsync;
        private readonly IRepositoryAsync<Bank> _bankRepositoryAsync;
        private readonly IMapper _mapper;
        private readonly IOutputCacheStore _outputCacheStored;

        public UpdateInternalBankAccountCommandHandler(IRepositoryAsync<InternalBankAccount> repositoryAsync, IRepositoryAsync<Bank> bankRepositoryAsync,
            IMapper mapper, IOutputCacheStore outputCacheStored)
        {
            _repositoryAsync = repositoryAsync;
            _bankRepositoryAsync = bankRepositoryAsync;
            _mapper = mapper;
            _outputCacheStored = outputCacheStored;
        }

        public async Task<Response<InternalBankAccountDto>> Handle(UpdateInternalBankAccountCommand request, CancellationToken cancellationToken)
        {
            var checkInternalBankAccount = await _repositoryAsync.GetByIdAsync(request.Id);
            if (checkInternalBankAccount == null)
            {
                throw new KeyNotFoundException($"No se encontro la ciudad con id {request.Id}");
            }

            var checkBank = await _bankRepositoryAsync.GetByIdAsync(request.BankId);
            if (checkBank == null)
            {
                throw new KeyNotFoundException($"No se encontro el banco con id {request.BankId}");
            }

            if (request.AccountType != checkInternalBankAccount.AccountType)
            {
                throw new ApiException("No se puede cambiar el tipo de cuenta. Cree una cuenta nueva si necesita cambiar entre cuentas corrientes/ahorro y tarjetas de crédito.");
            }

            checkInternalBankAccount.Name = request.Name;
            checkInternalBankAccount.BankId = request.BankId;
            checkInternalBankAccount.AccountNumber = request.AccountNumber;
            checkInternalBankAccount.CurrentAmount = request.CurrentAmount;
            checkInternalBankAccount.CardLastFour = request.CardLastFour;

            await _repositoryAsync.UpdateAsync(checkInternalBankAccount);
            await _repositoryAsync.SaveChangesAsync();
            await _outputCacheStored.EvictByTagAsync("cache_internalBankAccounts", cancellationToken);
            var dto = _mapper.Map<InternalBankAccountDto>(checkInternalBankAccount);
            return new Response<InternalBankAccountDto>(dto, $"{request.Name} actualizado correctamente");
        }
    }
}
