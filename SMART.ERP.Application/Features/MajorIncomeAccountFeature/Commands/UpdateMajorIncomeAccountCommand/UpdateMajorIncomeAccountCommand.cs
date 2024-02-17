using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.OutputCaching;
using SMART.ERP.Application.DTOs.MajorIncomeAccount;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.MajorIncomeAccountFeature.Commands.UpdateMajorIncomeAccountCommand
{
    public class UpdateMajorIncomeAccountCommand : IRequest<Response<MajorIncomeAccountDto>>
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
    }
    public class UpdateMajorIncomeAccountCommandHandler : IRequestHandler<UpdateMajorIncomeAccountCommand, Response<MajorIncomeAccountDto>>
    {
        private readonly IRepositoryAsync<MajorIncomeAccount> _repositoryAsync;
        private readonly IMapper _mapper;
        private readonly IOutputCacheStore _outputCacheStored;
        public UpdateMajorIncomeAccountCommandHandler(IRepositoryAsync<MajorIncomeAccount> repositoryAsync, IMapper mapper, IOutputCacheStore outputCacheStored)
        {
            _repositoryAsync = repositoryAsync;
            _mapper = mapper;
            _outputCacheStored = outputCacheStored;
        }
        public async Task<Response<MajorIncomeAccountDto>> Handle(UpdateMajorIncomeAccountCommand request, CancellationToken cancellationToken)
        {
            var checkAccount = await _repositoryAsync.GetByIdAsync(request.Id);
            if (checkAccount == null)
            {
                throw new KeyNotFoundException($"No se encontro la cuenta con id {request.Id}");
            }
            checkAccount.Name = request.Name;

            await _repositoryAsync.UpdateAsync(checkAccount);
            await _repositoryAsync.SaveChangesAsync();
            await _outputCacheStored.EvictByTagAsync("cache_major_income_account", cancellationToken);
            var dto = _mapper.Map<MajorIncomeAccountDto>(checkAccount);
            return new Response<MajorIncomeAccountDto>(dto, $"{request.Name} actualizada correctamente");
        }
    }
}
