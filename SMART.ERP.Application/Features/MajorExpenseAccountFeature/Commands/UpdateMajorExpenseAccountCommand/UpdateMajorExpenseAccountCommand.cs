using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.OutputCaching;
using SMART.ERP.Application.DTOs.MajorExpenseAccount;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;


namespace SMART.ERP.Application.Features.MajorExpenseAccountFeature.Commands.UpdateMajorExpenseAccountCommand
{
    public class UpdateMajorExpenseAccountCommand : IRequest<Response<MajorExpenseAccountDto>>
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
    }
    public class UpdateMajorExpenseAccountCommandHandle : IRequestHandler<UpdateMajorExpenseAccountCommand, Response<MajorExpenseAccountDto>>
    {
        private readonly IRepositoryAsync<MajorExpenseAccount> _repositoryAsync;
        private readonly IMapper _mapper;
        private readonly IOutputCacheStore _outputCacheStored;
        public UpdateMajorExpenseAccountCommandHandle(IRepositoryAsync<MajorExpenseAccount> repositoryAsync, IMapper mapper, IOutputCacheStore outputCacheStored)
        {
            _repositoryAsync = repositoryAsync;
            _mapper = mapper;
            _outputCacheStored = outputCacheStored;
        }
        public async Task<Response<MajorExpenseAccountDto>> Handle(UpdateMajorExpenseAccountCommand request, CancellationToken cancellationToken)
        {
            var checkAccount = await _repositoryAsync.GetByIdAsync(request.Id);
            if (checkAccount == null)
            {
                throw new KeyNotFoundException($"No se encontro la cuenta con id {request.Id}");
            }
            checkAccount.Name = request.Name;

            await _repositoryAsync.UpdateAsync(checkAccount);
            await _repositoryAsync.SaveChangesAsync();
            await _outputCacheStored.EvictByTagAsync("cache_major_expense_account", cancellationToken);
            var dto = _mapper.Map<MajorExpenseAccountDto>(checkAccount);
            return new Response<MajorExpenseAccountDto>(dto, $"{request.Name} actualizada correctamente");
        }
    }
}
