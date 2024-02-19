using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.OutputCaching;
using SMART.ERP.Application.DTOs.IncomeAccount;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.IncomeAccountFeature.Commands.UpdateIncomeAccountCommand
{
    public class UpdateIncomeAccountCommand : IRequest<Response<IncomeAccountDto>>
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string AccountNumber { get; set; } = null!;
        public int MajorIncomeAccountId { get; set; }
    }
    public class UpdateIncomeAccountCommandHandler : IRequestHandler<UpdateIncomeAccountCommand, Response<IncomeAccountDto>>
    {
        private readonly IRepositoryAsync<IncomeAccount> _repositoryAsync;
        private readonly IMapper _mapper;
        private readonly IOutputCacheStore _outputCacheStored;
        public UpdateIncomeAccountCommandHandler(IRepositoryAsync<IncomeAccount> repositoryAsync, IMapper mapper, IOutputCacheStore outputCacheStored)
        {
            _repositoryAsync = repositoryAsync;
            _mapper = mapper;
            _outputCacheStored = outputCacheStored;
        }
        public async Task<Response<IncomeAccountDto>> Handle(UpdateIncomeAccountCommand request, CancellationToken cancellationToken)
        {
            var checkAccount = await _repositoryAsync.GetByIdAsync(request.Id);
            if (checkAccount == null)
            {
                throw new KeyNotFoundException($"No se encontro la cuenta con id {request.Id}");
            }
            checkAccount.Name = request.Name;
            checkAccount.AccountNumber = request.AccountNumber;
            checkAccount.MajorIncomeAccountId = request.MajorIncomeAccountId;
            await _repositoryAsync.UpdateAsync(checkAccount);
            await _repositoryAsync.SaveChangesAsync();
            await _outputCacheStored.EvictByTagAsync("cache_income_account", cancellationToken);
            var dto = _mapper.Map<IncomeAccountDto>(checkAccount);
            return new Response<IncomeAccountDto>(dto, $"{request.Name} actualizada correctamente");
        }
    }
}
