using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.PurchaseBill;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.ProviderSpecification;
using SMART.ERP.Application.Specifications.PurchaseBillSpecification;
using SMART.ERP.Application.Specifications.PurchaseOrderSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

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
    }
    public class CreatePurchaseBillCommandHandler : IRequestHandler<CreatePurchaseBillCommand,Response<PurchaseBillDto>>
    {
        private readonly IRepositoryAsync<PurchaseBill> _repositoryAsync;
        private readonly IRepositoryAsync<PurchaseOrder> _purchaseOrderRepositoryAsync;
        private readonly IRepositoryAsync<Provider> _providerRepositoryAsync;
        private readonly IRepositoryAsync<Prefix> _prefixRepositoryAsync;
        private readonly IRepositoryAsync<ExpenseAccount> _expenseAccountRepositoryAsync;
        private readonly IMapper _mapper;
        public CreatePurchaseBillCommandHandler(IRepositoryAsync<PurchaseBill> repositoryAsync, IRepositoryAsync<ExpenseAccount> expenseAccountRepositoryAsync, IRepositoryAsync<Prefix> prefixRepositoryAsync, IRepositoryAsync<PurchaseOrder> purchaseOrderRepositoryAsync, IRepositoryAsync<Provider> providerRepositoryAsync, IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _purchaseOrderRepositoryAsync = purchaseOrderRepositoryAsync;
            _providerRepositoryAsync = providerRepositoryAsync;
            _mapper = mapper;
            _prefixRepositoryAsync = prefixRepositoryAsync;
            _expenseAccountRepositoryAsync = expenseAccountRepositoryAsync;
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
            newRecord.Outstanding = newRecord.Total;
            newRecord.CreationDate = DateTime.Now;
            var purchaseBillResponse = await _repositoryAsync.AddAsync(newRecord);
            await _repositoryAsync.SaveChangesAsync();
            purchaseBillResponse.Provider = providerExist;
            var dto = _mapper.Map<PurchaseBillDto>(purchaseBillResponse);
            return new Response<PurchaseBillDto>(dto, $"Factura de compra creada exitosamente.");
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
