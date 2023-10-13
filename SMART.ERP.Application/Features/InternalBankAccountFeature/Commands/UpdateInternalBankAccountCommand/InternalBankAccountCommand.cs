using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Address;
using SMART.ERP.Application.DTOs.InternalBankAccount;
using SMART.ERP.Application.Features.CityFeature.Commands.UpdateCityCommand;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.ERP.Application.Features.InternalBankAccountFeature.Commands.UpdateInternalBankAccountCommand
{
    public class UpdateInternalBankAccountCommand : IRequest<Response<InternalBankAccountDto>>
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int BankId { get; set; }
        public string AccountNumber { get; set; } = null!;
        public decimal CurrentAmount { get; set; }
    }

    public class UpdateInternalBankAccountCommandHandler : IRequestHandler<UpdateInternalBankAccountCommand, Response<InternalBankAccountDto>>
    {
        private readonly IRepositoryAsync<InternalBankAccount> _repositoryAsync;
        private readonly IRepositoryAsync<Bank> _bankRepositoryAsync;
        private readonly IMapper _mapper;

        public UpdateInternalBankAccountCommandHandler(IRepositoryAsync<InternalBankAccount> repositoryAsync, IRepositoryAsync<Bank> bankRepositoryAsync,
            IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _bankRepositoryAsync = bankRepositoryAsync;
            _mapper = mapper;
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

            checkInternalBankAccount.Name = request.Name;
            checkInternalBankAccount.BankId = request.BankId;
            checkInternalBankAccount.AccountNumber = request.AccountNumber;
            checkInternalBankAccount.CurrentAmount = request.CurrentAmount;

            await _repositoryAsync.UpdateAsync(checkInternalBankAccount);
            await _repositoryAsync.SaveChangesAsync();

            var dto = _mapper.Map<InternalBankAccountDto>(checkInternalBankAccount);
            return new Response<InternalBankAccountDto>(dto, $"{request.Name} actualizado correctamente");
        }
    }
}
