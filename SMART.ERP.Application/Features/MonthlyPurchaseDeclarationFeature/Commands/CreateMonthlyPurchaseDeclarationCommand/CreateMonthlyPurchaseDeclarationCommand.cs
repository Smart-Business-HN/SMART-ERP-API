using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.OutputCaching;
using SMART.ERP.Application.DTOs.MonthlyPurchaseDeclaration;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.JwtService;
using SMART.ERP.Application.Specifications.PurchaseBillSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.MonthlyPurchaseDeclarationFeature.Commands.CreateMonthlyPurchaseDeclarationCommand
{
    public class CreateMonthlyPurchaseDeclarationCommand : IRequest<Response<MonthlyPurchaseDeclarationDto>>
    {
        public DateTime DeclarationDate { get; set; }
    }
    public class CreateMonthlyPurchaseDeclarationCommandHandler : IRequestHandler<CreateMonthlyPurchaseDeclarationCommand, Response<MonthlyPurchaseDeclarationDto>>
    {
        private readonly IRepositoryAsync<MonthlyPurchaseDeclaration> _repositoryAsync;
        private readonly IRepositoryAsync<PurchaseBill> _purchaseBillRepositoryAsync;
        private readonly IRepositoryAsync<DeclaratedPurchaseBill> _declaratedPurchaseBillRepositoryAsync;
        private readonly IJwtService _jwtService;
        private readonly IMapper _mapper;
        private readonly IOutputCacheStore _outputCacheStored;
        public CreateMonthlyPurchaseDeclarationCommandHandler(IOutputCacheStore outputCacheStored, IRepositoryAsync<PurchaseBill> purchaseBillRepository, IRepositoryAsync<MonthlyPurchaseDeclaration> repositoryAsync, IRepositoryAsync<DeclaratedPurchaseBill> declaratedPurchaseBillRepositoryAsync, IJwtService jwtService, IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _declaratedPurchaseBillRepositoryAsync = declaratedPurchaseBillRepositoryAsync;
            _jwtService = jwtService;
            _mapper = mapper;
            _purchaseBillRepositoryAsync = purchaseBillRepository;
            _outputCacheStored = outputCacheStored;
        }
        public async Task<Response<MonthlyPurchaseDeclarationDto>> Handle(CreateMonthlyPurchaseDeclarationCommand request, CancellationToken cancellationToken)
        {
            var purchaseBills = await _purchaseBillRepositoryAsync.ListAsync(new FilterPurchaseBillByMonthAndYearSpecification(request.DeclarationDate.Month, request.DeclarationDate.Year));
            if (purchaseBills.Count == 0)
            {
                return new Response<MonthlyPurchaseDeclarationDto>("No hay facturas de compra para declarar en el mes.");
            }
            var declaratedPurchaseBills = new List<DeclaratedPurchaseBill>();
            foreach (var purchaseBill in purchaseBills)
            {
                var declaratedPurchaseBill = new DeclaratedPurchaseBill
                {
                    MonthlyPurchaseDeclarationId = 1,
                    PurchaseBillId = purchaseBill.Id,
                    ProviderRTN = purchaseBill.Provider!.RTN.Replace("-", ""),
                    ProviderName = purchaseBill.Provider.Name,
                    BillDate = purchaseBill.InvoiceDate.ToString("dd/MM/yyyy"),
                    Cai = purchaseBill.Cai,
                    Establishment = purchaseBill.InvoiceNumber.Substring(0, 3),
                    EmissionPoint = purchaseBill.InvoiceNumber.Substring(4, 3),
                    KindOfDocument = purchaseBill.InvoiceNumber.Substring(8, 2),
                    Correlative = purchaseBill.InvoiceNumber.Substring(11, 8),
                    PurchaseWithOce = "NO",
                    ResolutionNumber = null,
                    ResolutionDate = null,
                    Exempt = purchaseBill.Exempt,
                    Exonerated = purchaseBill.Exonerated,
                    TaxedAt15Percent = purchaseBill.TaxedAt15Percent,
                    TaxedAt18Percent = purchaseBill.TaxedAt18Percent,
                    Taxes15Percent = purchaseBill.Taxes15Percent,
                    Taxes18Percent = purchaseBill.Taxes18Percent
                };
                declaratedPurchaseBills.Add(declaratedPurchaseBill);
            }
            var month = request.DeclarationDate.Month.ToString().Length == 1 ? $"0{request.DeclarationDate.Month}" : request.DeclarationDate.Month.ToString();
            var monthlyPurchaseDeclaration = new MonthlyPurchaseDeclaration
            {
                Period = $"{request.DeclarationDate.Year}{month}",
                CreationDate = DateTime.Now,
                CreatedBy = _jwtService.GetSubjectToken(),
                StatusId = 31,
                TotalPurchaseBills = purchaseBills.Count,
                Exempt = purchaseBills.Sum(x => x.Exempt),
                Exonerated = purchaseBills.Sum(x => x.Exonerated),
                TaxedAt15Percent = purchaseBills.Sum(x => x.TaxedAt15Percent),
                TaxedAt18Percent = purchaseBills.Sum(x => x.TaxedAt18Percent),
                Taxes15Percent = purchaseBills.Sum(x => x.Taxes15Percent),
                Taxes18Percent = purchaseBills.Sum(x => x.Taxes18Percent),
                TotalTaxes = purchaseBills.Sum(x => x.Taxes15Percent + x.Taxes18Percent),
                Total = purchaseBills.Sum(x => x.Total),
            };
            var response = await _repositoryAsync.AddAsync(monthlyPurchaseDeclaration);
            declaratedPurchaseBills.ForEach(x => x.MonthlyPurchaseDeclarationId = response.Id);
            foreach (var declaratedPurchaseBill in declaratedPurchaseBills)
            {
                await _declaratedPurchaseBillRepositoryAsync.AddAsync(declaratedPurchaseBill);
            }
            await _repositoryAsync.SaveChangesAsync();
            await _declaratedPurchaseBillRepositoryAsync.SaveChangesAsync();
            await _outputCacheStored.EvictByTagAsync("cache_monthlyPurchaseDeclaration", cancellationToken);
            var dto = _mapper.Map<MonthlyPurchaseDeclarationDto>(response);
            return new Response<MonthlyPurchaseDeclarationDto>(dto);
        }
    }
}
