using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.OutputCaching;
using SMART.ERP.Application.DTOs.ExpenseAccount;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.ExpenseAccountSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.ExpenseAccountFeature.Commands.CreateExpenseAccountCommand
{
    public class CreateExpenseAccountCommand : IRequest<Response<ExpenseAccountDto>>
    {
        public string Name { get; set; } = null!;
        public string AccountNumber { get; set; } = null!;
        public int MajorExpenseAccountId { get; set; }
    }
    public class CreateExpenseAccountCommandHandler : IRequestHandler<CreateExpenseAccountCommand, Response<ExpenseAccountDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<ExpenseAccount> _repositoryAsync;
        private readonly IOutputCacheStore _outputCacheStored;
        public CreateExpenseAccountCommandHandler(IMapper mapper, IOutputCacheStore outputCacheStore, IRepositoryAsync<ExpenseAccount> repositoryAsync)
        {
            _mapper = mapper;
            _outputCacheStored = outputCacheStore;
            _repositoryAsync = repositoryAsync;
        }
        public async Task<Response<ExpenseAccountDto>> Handle(CreateExpenseAccountCommand request, CancellationToken cancellationToken)
        {
            var checkbyName = await _repositoryAsync.FirstOrDefaultAsync(new FilterExpenseAccountFromNameSpecification(request.Name));
            if (checkbyName != null)
            {
                throw new ApiException($"Ya existe una cuenta con el nombre {request.Name}");
            }
            var newRecord = _mapper.Map<ExpenseAccount>(request);
            var response = await _repositoryAsync.AddAsync(newRecord);
            await _repositoryAsync.SaveChangesAsync();
            await _outputCacheStored.EvictByTagAsync("cache_expense_account", cancellationToken);
            var dto = _mapper.Map<ExpenseAccountDto>(response);
            return new Response<ExpenseAccountDto>(dto, $"{request.Name} agregada correctamente");
        }
    }
}
