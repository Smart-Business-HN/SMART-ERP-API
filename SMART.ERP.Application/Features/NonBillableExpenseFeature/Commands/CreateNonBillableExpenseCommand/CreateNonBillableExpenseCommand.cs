using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.OutputCaching;
using SMART.ERP.Application.DTOs.NonBilllableExpense;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.AccountingPostingService;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Domain.Enums;

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
        public int? ProjectId { get; set; }
        /// <summary>Tipo de retención (None, ISR 12.5%, ISR 1%, ISV 15% Art.13).</summary>
        public WithholdingType WithholdingType { get; set; } = WithholdingType.None;
    }

    public class CreateNonBillableExpenseCommandHandler : IRequestHandler<CreateNonBillableExpenseCommand, Response<NonBillableExpenseDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<NonBillableExpense> _repositoryAsync;
        private readonly IRepositoryAsync<Provider> _providerRepositoryAsync;
        private readonly IRepositoryAsync<Prefix> _prefixRepositoryAsync;
        private readonly IRepositoryAsync<ExpenseAccount> _expenseAccountRepositoryAsync;
        private readonly IOutputCacheStore _outputCacheStored;
        private readonly IAccountingPostingService _accountingPostingService;

        public CreateNonBillableExpenseCommandHandler(
            IMapper mapper,
            IRepositoryAsync<NonBillableExpense> repositoryAsync,
            IRepositoryAsync<Provider> providerRepositoryAsync,
            IRepositoryAsync<Prefix> prefixRepositoryAsync,
            IRepositoryAsync<ExpenseAccount> expenseAccountRepositoryAsync,
            IOutputCacheStore outputCacheStored,
            IAccountingPostingService accountingPostingService)
        {
            _mapper = mapper;
            _repositoryAsync = repositoryAsync;
            _outputCacheStored = outputCacheStored;
            _providerRepositoryAsync = providerRepositoryAsync;
            _prefixRepositoryAsync = prefixRepositoryAsync;
            _expenseAccountRepositoryAsync = expenseAccountRepositoryAsync;
            _accountingPostingService = accountingPostingService;
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
            newRecord.WithholdingType = request.WithholdingType;
            var (whBase, whAmount) = CalculateWithholding(request.WithholdingType, request.Amount);
            newRecord.WithholdingBase = whBase;
            newRecord.WithholdingAmount = whAmount;
            newRecord.Outstanding = request.Amount - whAmount;
            newRecord.StatusId = 29;
            newRecord.ExpenseCode = CreateNonBillableExpenseCode(prefixExist, currentNonBillableExpense.Last());
            var data = await _repositoryAsync.AddAsync(newRecord);
            await _repositoryAsync.SaveChangesAsync();
            await _accountingPostingService.PostNonBillableExpenseAsync(data.Id, cancellationToken);
            await _outputCacheStored.EvictByTagAsync("cache_nonBillableExpense", cancellationToken);
            data.Prefix = prefixExist;
            data.Provider = providerExist;
            data.ExpenseAccount = expenseAccountExist;
            var dto = _mapper.Map<NonBillableExpenseDto>(data);
            
            return new Response<NonBillableExpenseDto>(dto, message: $"Nuevo recibo de gasto creado exitosamente");
        }
        /// <summary>
        /// Calcula (base, monto) de retención sobre un gasto no facturable (sin ISV).
        /// La base es el Amount completo; ISR 12.5% u 1% según tipo. ISV 15% no aplica a gastos sin ISV.
        /// </summary>
        public static (decimal Base, decimal Amount) CalculateWithholding(WithholdingType type, decimal amount)
        {
            if (type == WithholdingType.None) return (0m, 0m);
            decimal rate = type switch
            {
                WithholdingType.ISR12_5 => 0.125m,
                WithholdingType.ISR1 => 0.01m,
                _ => 0m // ISV 15% no aplica en gastos no facturables
            };
            if (rate == 0m) return (0m, 0m);
            return (amount, Math.Round(amount * rate, 2, MidpointRounding.AwayFromZero));
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
