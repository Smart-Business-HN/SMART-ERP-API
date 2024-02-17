using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.OutputCaching;
using SMART.ERP.Application.DTOs.IncomeAccount;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.IncomeAccountSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.IncomeAccountFeature.Commands.CreateIncomeAccountCommand
{
    public class CreateIncomeAccountCommand : IRequest<Response<IncomeAccountDto>>
    {
        public string Name { get; set; } = null!;
        public string AccountNumber { get; set; } = null!;
        public int MajorExpenseAccountId { get; set; }
    }
    public class CreateIncomeAccountCommandHandler : IRequestHandler<CreateIncomeAccountCommand, Response<IncomeAccountDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<IncomeAccount> _repositoryAsync;
        private readonly IOutputCacheStore _outputCacheStored;
        public CreateIncomeAccountCommandHandler(IMapper mapper, IOutputCacheStore outputCacheStore, IRepositoryAsync<IncomeAccount> repositoryAsync)
        {
            _mapper = mapper;
            _outputCacheStored = outputCacheStore;
            _repositoryAsync = repositoryAsync;
        }
        public async Task<Response<IncomeAccountDto>> Handle(CreateIncomeAccountCommand request, CancellationToken cancellationToken)
        {
            var checkbyName = await _repositoryAsync.FirstOrDefaultAsync(new FilterIncomeAccountFromNameSpecification(request.Name));
            if (checkbyName != null)
            {
                throw new ApiException($"Ya existe una cuenta con el nombre {request.Name}");
            }
            var newRecord = _mapper.Map<IncomeAccount>(request);
            var response = await _repositoryAsync.AddAsync(newRecord);
            await _repositoryAsync.SaveChangesAsync();
            await _outputCacheStored.EvictByTagAsync("cache_income_account", cancellationToken);
            var dto = _mapper.Map<IncomeAccountDto>(response);
            return new Response<IncomeAccountDto>(dto, $"{request.Name} agregada correctamente");
        }
    }

}
