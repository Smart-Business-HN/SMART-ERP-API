using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.PurchaseBill;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.AccountingPostingService;
using SMART.ERP.Application.Specifications.ProviderSpecification;
using SMART.ERP.Application.Specifications.PurchaseBillSpecification;
using SMART.ERP.Application.Specifications.PurchaseOrderSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Domain.Enums;

namespace SMART.ERP.Application.Features.PurchaseBillFeature.Commands.CreatePurchaseBillCommand
{
    public class CreatePurchaseBillCommand : IRequest<Response<PurchaseBillDto>>
    {
        public int ProviderId { get; set; }
        public string InvoiceNumber { get; set; } = null!;
        public DateTime InvoiceDate { get; set; }
        public string Cai { get; set; } = null!;
        public int? PurchaseOrderOriginId { get; set; }
        public decimal? Exempt { get; set; }
        public decimal? Exonerated { get; set; }
        public decimal? TaxedAt15Percent { get; set; }
        public decimal? TaxedAt18Percent { get; set; }
        public decimal? Taxes15Percent { get; set; }
        public decimal? Taxes18Percent { get; set; }
        public int PrefixId { get; set; }
        public int ExpenseAccountId { get; set; }
        public int? ProjectId { get; set; }
        /// <summary>Tipo de retención que el comprador aplica al proveedor (None = no aplica).</summary>
        public WithholdingType WithholdingType { get; set; } = WithholdingType.None;
    }
    public class CreatePurchaseBillCommandHandler : IRequestHandler<CreatePurchaseBillCommand,Response<PurchaseBillDto>>
    {
        private readonly IRepositoryAsync<PurchaseBill> _repositoryAsync;
        private readonly IRepositoryAsync<PurchaseOrder> _purchaseOrderRepositoryAsync;
        private readonly IRepositoryAsync<Provider> _providerRepositoryAsync;
        private readonly IRepositoryAsync<Prefix> _prefixRepositoryAsync;
        private readonly IRepositoryAsync<ExpenseAccount> _expenseAccountRepositoryAsync;
        private readonly IMapper _mapper;
        private readonly IAccountingPostingService _accountingPostingService;
        public CreatePurchaseBillCommandHandler(IRepositoryAsync<PurchaseBill> repositoryAsync, IRepositoryAsync<ExpenseAccount> expenseAccountRepositoryAsync, IRepositoryAsync<Prefix> prefixRepositoryAsync, IRepositoryAsync<PurchaseOrder> purchaseOrderRepositoryAsync, IRepositoryAsync<Provider> providerRepositoryAsync, IMapper mapper, IAccountingPostingService accountingPostingService)
        {
            _repositoryAsync = repositoryAsync;
            _purchaseOrderRepositoryAsync = purchaseOrderRepositoryAsync;
            _providerRepositoryAsync = providerRepositoryAsync;
            _mapper = mapper;
            _prefixRepositoryAsync = prefixRepositoryAsync;
            _expenseAccountRepositoryAsync = expenseAccountRepositoryAsync;
            _accountingPostingService = accountingPostingService;
        }
        public async Task<Response<PurchaseBillDto>> Handle(CreatePurchaseBillCommand request, CancellationToken cancellationToken)
        {
            if(request.PurchaseOrderOriginId != null)
            {
                var purchaseOrderExist = await _purchaseOrderRepositoryAsync.FirstOrDefaultAsync(new FilterPurchaseOrderByIdSpecification((int)request.PurchaseOrderOriginId));
                if (purchaseOrderExist == null)
                {
                    throw new ApiException($"No existe una orden de compra con el Id {request.PurchaseOrderOriginId}");
                }
            }
            var providerExist = await _providerRepositoryAsync.FirstOrDefaultAsync(new FilterProviderSpecification(null,request.ProviderId));
            if (providerExist == null)
            {
                throw new ApiException($"No existe una proveedor con el Id {request.ProviderId}");
            }
            var prefixExist = await _prefixRepositoryAsync.GetByIdAsync(request.PrefixId);
            if (prefixExist == null)
            {
                throw new ApiException($"No existe un prefijo con el id {request.PrefixId}");
            }
            var expenseAccountExist = await _expenseAccountRepositoryAsync.GetByIdAsync(request.ExpenseAccountId);
            if (expenseAccountExist == null)
            {
                throw new ApiException($"No existe una cuenta de gastos con el id {request.PrefixId}");
            }
            var checkIfPurchaseBillExist = await _repositoryAsync.ListAsync(new FilterPurchaseBillByExistingValuesSpecification(request));
            if(checkIfPurchaseBillExist.Count() > 0)
            {
                throw new ApiException($"Ya existe una factura con este numero, CAI y/o proveedor registrada. Favor revisar factura con ID {checkIfPurchaseBillExist[0].Id}");
            }
            var currentPurchaseBills = await _repositoryAsync.ListAsync();
            var newRecord = _mapper.Map<PurchaseBill>(request);
            newRecord.PurchaseBillCode = CreatePurchaseBillCode(prefixExist, currentPurchaseBills.Last());
            newRecord.ProviderId = request.ProviderId;
            newRecord.PurchaseOrderOriginId = request.PurchaseOrderOriginId;
            newRecord.ExpenseAccountId = request.ExpenseAccountId;
            newRecord.StatusId = 27;
            newRecord.Total = (decimal)(request.Exempt + request.Exonerated + request.Taxes15Percent + request.Taxes18Percent + request.TaxedAt15Percent + request.TaxedAt18Percent)!;

            // Retenciones: base = neto sin ISV, monto = base × tasa. El neto a pagar al proveedor disminuye.
            newRecord.WithholdingType = request.WithholdingType;
            var (whBase, whAmount) = CalculateWithholding(request.WithholdingType, newRecord);
            newRecord.WithholdingBase = whBase;
            newRecord.WithholdingAmount = whAmount;
            newRecord.Outstanding = newRecord.Total - whAmount;
            newRecord.CreationDate = DateTime.Now;
            var purchaseBillResponse = await _repositoryAsync.AddAsync(newRecord);
            await _repositoryAsync.SaveChangesAsync();
            await _accountingPostingService.PostPurchaseBillAsync(purchaseBillResponse.Id, cancellationToken);
            purchaseBillResponse.Provider = providerExist;
            var dto = _mapper.Map<PurchaseBillDto>(purchaseBillResponse);
            return new Response<PurchaseBillDto>(dto, $"Factura de compra creada exitosamente.");
        }
        /// <summary>
        /// Calcula (base, monto) de retención según tipo. Base = neto sin ISV (TaxedAt15 + TaxedAt18 + Exempt + Exonerated).
        /// Las tasas: ISR 12.5% honorarios, ISR 1% bienes/servicios, ISV 15% Art.13 sobre el ISV pagado.
        /// </summary>
        public static (decimal Base, decimal Amount) CalculateWithholding(WithholdingType type, PurchaseBill bill)
        {
            if (type == WithholdingType.None) return (0m, 0m);
            var netBase = bill.TaxedAt15Percent + bill.TaxedAt18Percent + bill.Exempt + bill.Exonerated;
            decimal rate = type switch
            {
                WithholdingType.ISR12_5 => 0.125m,
                WithholdingType.ISR1 => 0.01m,
                WithholdingType.ISV15 => 0m, // se calcula sobre el ISV recibido, no sobre el neto
                _ => 0m
            };
            if (type == WithholdingType.ISV15)
            {
                // Honduras Art. 13: el comprador retiene el ISV 15% que aparece como crédito fiscal en la factura.
                var amount = Math.Round(bill.Taxes15Percent, 2, MidpointRounding.AwayFromZero);
                return (bill.Taxes15Percent, amount);
            }
            return (netBase, Math.Round(netBase * rate, 2, MidpointRounding.AwayFromZero));
        }

        public static string CreatePurchaseBillCode(Prefix prefix, PurchaseBill lastPurchaseBill)
        {
            var numberOfCharacters = prefix.Format.ToCharArray().Length;
            var numberOfCharactersInId = lastPurchaseBill.Id.ToString().ToCharArray().Length;
            var code = "";
            if (numberOfCharacters + numberOfCharactersInId < 8)
            {
                int characterOffset = 8 - (numberOfCharacters + numberOfCharactersInId);
                code = prefix.Format + new string('0', characterOffset) + (lastPurchaseBill.Id + 1).ToString();
            }
            else
            {
                code = prefix.Format + (lastPurchaseBill.Id + 1).ToString();
            }
            return code;
        }
    }
}
