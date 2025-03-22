using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.OutputCaching;
using SMART.ERP.Application.DTOs.NonBilllableExpense;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.NonBillableExpenseFeature.Commands.CreateNonBillableExpenseCommand
{
    public class CreateNonBillableExpenseCommand : IRequest<Response<NonBillableExpenseDto>>
    {
        public int ProviderId { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; } = null!;
        public int ExpenseAccountId { get; set; }
        public int PrefixId { get; set; }
    }

    public class CreateNonBillableExpenseCommandHandler : IRequestHandler<CreateNonBillableExpenseCommand, Response<NonBillableExpenseDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<NonBillableExpense> _repositoryAsync;
        private readonly IRepositoryAsync<Provider> _providerRepositoryAsync;
        private readonly IRepositoryAsync<Prefix> _prefixRepositoryAsync;
        private readonly IRepositoryAsync<ExpenseAccount> _expenseAccountRepositoryAsync;
        private readonly IOutputCacheStore _outputCacheStored;

        public CreateNonBillableExpenseCommandHandler(
            IMapper mapper,
            IRepositoryAsync<NonBillableExpense> repositoryAsync,
            IRepositoryAsync<Provider> providerRepositoryAsync,
            IRepositoryAsync<Prefix> prefixRepositoryAsync,
            IRepositoryAsync<ExpenseAccount> expenseAccountRepositoryAsync,
            IOutputCacheStore outputCacheStored)
        {
            _mapper = mapper;
            _repositoryAsync = repositoryAsync;
            _outputCacheStored = outputCacheStored;
            _providerRepositoryAsync = providerRepositoryAsync;
            _prefixRepositoryAsync = prefixRepositoryAsync;
            _expenseAccountRepositoryAsync = expenseAccountRepositoryAsync;
        }

        public async Task<Response<NonBillableExpenseDto>> Handle(CreateNonBillableExpenseCommand request, CancellationToken cancellationToken)
        {
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
            var currentNonBillableExpense = await _repositoryAsync.ListAsync();
            if (currentNonBillableExpense.Count() == 0)
            {
                var firstNonBillableExpense = new NonBillableExpense { Id = 0 };
                currentNonBillableExpense = [firstNonBillableExpense];
            }

            var newRecord = _mapper.Map<NonBillableExpense>(request);
            newRecord.Outstanding = request.Amount;
            newRecord.StatusId = 29;
            newRecord.ExpenseCode = CreateNonBillableExpenseCode(prefixExist, currentNonBillableExpense.Last());
            var data = await _repositoryAsync.AddAsync(newRecord);
            await _repositoryAsync.SaveChangesAsync();
            await _outputCacheStored.EvictByTagAsync("cache_nonBillableExpense", cancellationToken);
            data.Prefix = prefixExist;
            data.Provider = providerExist;
            data.ExpenseAccount = expenseAccountExist;
            var dto = _mapper.Map<NonBillableExpenseDto>(data);
            
            return new Response<NonBillableExpenseDto>(dto, message: $"Nuevo recibo de gasto creado exitosamente");
        }
        public static string CreateNonBillableExpenseCode(Prefix prefix, NonBillableExpense lastNonBillableExpense)
        {
            var numberOfCharacters = prefix.Format.ToCharArray().Length;
            var numberOfCharactersInId = (lastNonBillableExpense.Id + 1).ToString().ToCharArray().Length;
            var code = "";
            if (numberOfCharacters + numberOfCharactersInId < 8)
            {
                int characterOffset = 8 - (numberOfCharacters + numberOfCharactersInId);
                code = prefix.Format + new string('0', characterOffset) + (lastNonBillableExpense.Id + 1).ToString();
            }
            else
            {
                code = prefix.Format + (lastNonBillableExpense.Id + 1).ToString();
            }
            return code;
        }
    }
}
