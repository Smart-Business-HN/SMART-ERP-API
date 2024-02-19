using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.OutputCaching;
using SMART.ERP.Application.DTOs.ExpenseAccount;
using SMART.ERP.Application.DTOs.IncomeAccount;
using SMART.ERP.Application.Features.IncomeAccountFeature.Commands.UpdateIncomeAccountCommand;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.ERP.Application.Features.ExpenseAccountFeature.Commands.UpdateExpenseAccountCommand
{
    public class UpdateExpenseAccountCommand : IRequest<Response<ExpenseAccountDto>>
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string AccountNumber { get; set; } = null!;
        public int MajorExpenseAccountId { get; set; }
    }
    public class UpdateExpenseAccountCommandHandler : IRequestHandler<UpdateExpenseAccountCommand, Response<ExpenseAccountDto>>
    {
        private readonly IRepositoryAsync<ExpenseAccount> _repositoryAsync;
        private readonly IMapper _mapper;
        private readonly IOutputCacheStore _outputCacheStored;
        public UpdateExpenseAccountCommandHandler(IRepositoryAsync<ExpenseAccount> repositoryAsync, IMapper mapper, IOutputCacheStore outputCacheStored)
        {
            _repositoryAsync = repositoryAsync;
            _mapper = mapper;
            _outputCacheStored = outputCacheStored;
        }
        public async Task<Response<ExpenseAccountDto>> Handle(UpdateExpenseAccountCommand request, CancellationToken cancellationToken)
        {
            var checkAccount = await _repositoryAsync.GetByIdAsync(request.Id);
            if (checkAccount == null)
            {
                throw new KeyNotFoundException($"No se encontro la cuenta con id {request.Id}");
            }
            checkAccount.Name = request.Name;
            checkAccount.AccountNumber = request.AccountNumber;
            checkAccount.MajorExpenseAccountId = request.MajorExpenseAccountId;
            await _repositoryAsync.UpdateAsync(checkAccount);
            await _repositoryAsync.SaveChangesAsync();
            await _outputCacheStored.EvictByTagAsync("cache_income_account", cancellationToken);
            var dto = _mapper.Map<ExpenseAccountDto>(checkAccount);
            return new Response<ExpenseAccountDto>(dto, $"{request.Name} actualizada correctamente");
        }
    }
}
