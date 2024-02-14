using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.OutputCaching;
using SMART.ERP.Application.DTOs.Bank;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;


namespace SMART.ERP.Application.Features.BankFeature.Commands.UpdateBankCommand
{
    public class UpdateBankCommand : IRequest<Response<BankDto>>
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public bool ItIsNationalBank { get; set; }
        public bool IsActive { get; set; }
    }

    public class UpdateBankCommandHandler : IRequestHandler<UpdateBankCommand, Response<BankDto>>
    {
        private readonly IRepositoryAsync<Bank> _repositoryAsync;
        private readonly IMapper _mapper;
        private readonly IOutputCacheStore _outputCacheStored;
        public UpdateBankCommandHandler(IRepositoryAsync<Bank> repositoryAsync,
            IMapper mapper, IOutputCacheStore outputCacheStore)
        {
            _repositoryAsync = repositoryAsync;
            _mapper = mapper;
            _outputCacheStored = outputCacheStore;
        }

        public async Task<Response<BankDto>> Handle(UpdateBankCommand request, CancellationToken cancellationToken)
        {
            var checkBank = await _repositoryAsync.GetByIdAsync(request.Id);
            if (checkBank == null)
            {
                throw new KeyNotFoundException($"No se encontro el banco con id {request.Id}");
            }

            checkBank.Name = request.Name;
            checkBank.ItIsNationalBank = request.ItIsNationalBank;
            checkBank.IsActive = request.IsActive;

            await _repositoryAsync.UpdateAsync(checkBank);
            await _repositoryAsync.SaveChangesAsync();
            await _outputCacheStored.EvictByTagAsync("cache_banks", cancellationToken);
            var dto = _mapper.Map<BankDto>(checkBank);
            return new Response<BankDto>(dto, $"{request.Name} actualizado correctamente");
        }
    }
}
