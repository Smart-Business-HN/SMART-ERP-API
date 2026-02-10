using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.OutputCaching;
using SMART.ERP.Application.DTOs.MonthlySaleDeclaration;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.JwtService;
using SMART.ERP.Application.Specifications.InvoiceSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.MonthlySaleDeclarationFeature.Commands.CreateMonthlySaleDeclarationCommand
{
    public class CreateMonthlySaleDeclarationCommand : IRequest<Response<MonthlySaleDeclarationDto>>
    {
        public DateTime DeclarationDate { get; set; }
    }
    public class CreateMonthlySaleDeclarationCommandHandler : IRequestHandler<CreateMonthlySaleDeclarationCommand, Response<MonthlySaleDeclarationDto>>
    {
        private readonly IRepositoryAsync<MonthlySaleDeclaration> _repositoryAsync;
        private readonly IRepositoryAsync<Invoice> _invoiceRepositoryAsync;
        private readonly IRepositoryAsync<DeclaredSaleInvoice> _declaredSaleInvoiceRepositoryAsync;
        private readonly IJwtService _jwtService;
        private readonly IMapper _mapper;
        private readonly IOutputCacheStore _outputCacheStored;
        public CreateMonthlySaleDeclarationCommandHandler(IOutputCacheStore outputCacheStored, IRepositoryAsync<Invoice> invoiceRepository, IRepositoryAsync<MonthlySaleDeclaration> repositoryAsync, IRepositoryAsync<DeclaredSaleInvoice> declaredSaleInvoiceRepositoryAsync, IJwtService jwtService, IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _declaredSaleInvoiceRepositoryAsync = declaredSaleInvoiceRepositoryAsync;
            _jwtService = jwtService;
            _mapper = mapper;
            _invoiceRepositoryAsync = invoiceRepository;
            _outputCacheStored = outputCacheStored;
        }
        public async Task<Response<MonthlySaleDeclarationDto>> Handle(CreateMonthlySaleDeclarationCommand request, CancellationToken cancellationToken)
        {
            var invoices = await _invoiceRepositoryAsync.ListAsync(new FilterInvoicesForSaleDeclarationSpecification(request.DeclarationDate.Month, request.DeclarationDate.Year));
            if (invoices.Count == 0)
            {
                return new Response<MonthlySaleDeclarationDto>("No hay facturas de venta para declarar en el mes.");
            }
            var declaredSaleInvoices = new List<DeclaredSaleInvoice>();
            foreach (var invoice in invoices)
            {
                var declaredSaleInvoice = new DeclaredSaleInvoice
                {
                    MonthlySaleDeclarationId = 1,
                    InvoiceId = invoice.Id,
                    CustomerRTN = invoice.Customer?.RTN?.Replace("-", "") ?? "",
                    CustomerName = invoice.Customer?.FullName ?? "",
                    InvoiceDate = invoice.CreationDate.ToString("dd/MM/yyyy"),
                    Cai = invoice.Cai?.Identificator ?? "",
                    CaiName = invoice.Cai?.Name ?? "",
                    Establishment = invoice.InvoiceNumber.Substring(0, 3),
                    EmissionPoint = invoice.InvoiceNumber.Substring(4, 3),
                    KindOfDocument = invoice.InvoiceNumber.Substring(8, 2),
                    Correlative = invoice.InvoiceNumber.Substring(11, 8),
                    SaleWithExoneration = string.IsNullOrEmpty(invoice.SagCode) ? "NO" : "SI",
                    Exempt = invoice.Exempt,
                    Exonerated = invoice.Exonerated,
                    TaxedAt15Percent = invoice.TaxedAt15Percent,
                    TaxedAt18Percent = invoice.TaxedAt18Percent,
                    Taxes15Percent = invoice.Taxes15Percent,
                    Taxes18Percent = invoice.Taxes18Percent
                };
                declaredSaleInvoices.Add(declaredSaleInvoice);
            }
            var month = request.DeclarationDate.Month.ToString().Length == 1 ? $"0{request.DeclarationDate.Month}" : request.DeclarationDate.Month.ToString();
            var monthlySaleDeclaration = new MonthlySaleDeclaration
            {
                Period = $"{request.DeclarationDate.Year}{month}",
                CreationDate = DateTime.Now,
                CreatedBy = _jwtService.GetSubjectToken(),
                StatusId = 31,
                TotalInvoices = invoices.Count,
                Exempt = invoices.Sum(x => x.Exempt),
                Exonerated = invoices.Sum(x => x.Exonerated),
                TaxedAt15Percent = invoices.Sum(x => x.TaxedAt15Percent),
                TaxedAt18Percent = invoices.Sum(x => x.TaxedAt18Percent),
                Taxes15Percent = invoices.Sum(x => x.Taxes15Percent),
                Taxes18Percent = invoices.Sum(x => x.Taxes18Percent),
                TotalTaxes = invoices.Sum(x => x.Taxes15Percent + x.Taxes18Percent),
                Total = invoices.Sum(x => x.Total),
            };
            var response = await _repositoryAsync.AddAsync(monthlySaleDeclaration);
            declaredSaleInvoices.ForEach(x => x.MonthlySaleDeclarationId = response.Id);
            foreach (var declaredSaleInvoice in declaredSaleInvoices)
            {
                await _declaredSaleInvoiceRepositoryAsync.AddAsync(declaredSaleInvoice);
            }
            await _repositoryAsync.SaveChangesAsync();
            await _declaredSaleInvoiceRepositoryAsync.SaveChangesAsync();
            await _outputCacheStored.EvictByTagAsync("cache_monthlySaleDeclaration", cancellationToken);
            var dto = _mapper.Map<MonthlySaleDeclarationDto>(response);
            return new Response<MonthlySaleDeclarationDto>(dto);
        }
    }
}
