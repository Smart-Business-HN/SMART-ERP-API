using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.OutputCaching;
using SMART.ERP.Application.DTOs.Bank;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.BankSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.BankFeature.Commands.CreateBankCommand
{
    public class CreateBankCommand : IRequest<Response<BankDto>>
    {
        public string Name { get; set; } = null!;
        public bool IsActive { get; set; }
        public bool ItIsNationalBank { get; set; }
    }

    public class CreateBankCommandHandler : IRequestHandler<CreateBankCommand, Response<BankDto>>
    {
        private readonly IRepositoryAsync<Bank> _repositoryAsync;
        private readonly IMapper _mapper;
        private readonly IOutputCacheStore _outputCacheStored;
        public CreateBankCommandHandler(IRepositoryAsync<Bank> repositoryAsync,
            IMapper mapper, IOutputCacheStore outputCacheStore)
        {
            _repositoryAsync = repositoryAsync;
            _mapper = mapper;
            _outputCacheStored = outputCacheStore;
        }

        public async Task<Response<BankDto>> Handle(CreateBankCommand request, CancellationToken cancellationToken)
        {
            var checkbyName = await _repositoryAsync.FirstOrDefaultAsync(new FilterBankFromNameSpecification(request.Name));
            if (checkbyName != null)
            {
                throw new ApiException($"Ya existe un banco con el nombre {request.Name}");
            }

            var newRecord = _mapper.Map<Bank>(request);
            var response = await _repositoryAsync.AddAsync(newRecord);
            await _repositoryAsync.SaveChangesAsync();
            await _outputCacheStored.EvictByTagAsync("cache_banks",cancellationToken);
            var dto = _mapper.Map<BankDto>(response);
            return new Response<BankDto>(dto, $"{request.Name} agregado correctamente");
        }
    }
}
