using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.OutputCaching;
using SMART.ERP.Application.DTOs.NonBilllableExpense;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.NonBillableExpenseFeature.Commands.UpdateNonBillableExpenseCommand
{
    public class UpdateNonBillableExpenseCommand : IRequest<Response<NonBillableExpenseDto>>
    {
        public int Id { get; set; }
        public int ProviderId { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; } = null!;
        public int ExpenseAccountId { get; set; }
        public int PrefixId { get; set; }
    }
    public class UpdateNonBillableExpenseCommandHandler : IRequestHandler<UpdateNonBillableExpenseCommand, Response<NonBillableExpenseDto>>
    {
        private readonly IRepositoryAsync<NonBillableExpense> _repositoryAsync;
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<Provider> _providerRepositoryAsync;
        private readonly IRepositoryAsync<Prefix> _prefixRepositoryAsync;
        private readonly IRepositoryAsync<ExpenseAccount> _expenseAccountRepositoryAsync;
        private readonly IOutputCacheStore _outputCacheStored;

        public UpdateNonBillableExpenseCommandHandler(IRepositoryAsync<NonBillableExpense> repositoryAsync, IMapper mapper, IRepositoryAsync<Provider> providerRepositoryAsync,
            IRepositoryAsync<Prefix> prefixRepositoryAsync,
            IRepositoryAsync<ExpenseAccount> expenseAccountRepositoryAsync, IOutputCacheStore outputCacheStored)
        {
            _repositoryAsync = repositoryAsync;
            _mapper = mapper;
            _outputCacheStored = outputCacheStored;
            _providerRepositoryAsync = providerRepositoryAsync;
            _prefixRepositoryAsync = prefixRepositoryAsync;
            _expenseAccountRepositoryAsync = expenseAccountRepositoryAsync;
        }
        public async Task<Response<NonBillableExpenseDto>> Handle(UpdateNonBillableExpenseCommand request, CancellationToken cancellationToken)
        {
            var nonBillableExpense = await _repositoryAsync.GetByIdAsync(request.Id);
            if (nonBillableExpense == null)
            {
                throw new KeyNotFoundException($"No se encontro ningun registro con el id {request.Id}");
            }
            var providerExist = await _providerRepositoryAsync.GetByIdAsync(request.ProviderId);
            if (providerExist == null)
            {
                throw new ApiException($"No existe un proveedor con el ID: {request.ProviderId}");
            }
            var prefixExist = await _prefixRepositoryAsync.GetByIdAsync(request.PrefixId);
            if (prefixExist == null)
            {
                throw new ApiException($"No existe un prefijo con el ID: {request.PrefixId}");
            }
            var expenseAccountExist = await _expenseAccountRepositoryAsync.GetByIdAsync(request.ExpenseAccountId);
            if (expenseAccountExist == null)
            {
                throw new ApiException($"No existe una cuenta de gastos con el ID: {request.PrefixId}");
            }
            nonBillableExpense.Description = request.Description;
            nonBillableExpense.ProviderId = request.ProviderId;
            nonBillableExpense.Date = request.Date;
            nonBillableExpense.ExpenseAccountId = request.ExpenseAccountId;
            nonBillableExpense.Amount = request.Amount;
            nonBillableExpense.Outstanding = request.Amount;
            await _repositoryAsync.UpdateAsync(nonBillableExpense);
            await _repositoryAsync.SaveChangesAsync();
            await _outputCacheStored.EvictByTagAsync("cache_nonBillableExpense", cancellationToken);
            var dto = _mapper.Map<NonBillableExpenseDto>(nonBillableExpense);
            return new Response<NonBillableExpenseDto>(dto, message: $"Gasto no declarable actualizado correctamente.");

        }
    }
}
