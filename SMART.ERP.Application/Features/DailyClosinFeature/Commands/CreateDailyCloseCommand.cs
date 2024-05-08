using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.OutputCaching;
using SMART.ERP.Application.DTOs.DailyClose;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.JwtService;
using SMART.ERP.Application.Specifications.InvoiceSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.DailyClosinFeature.Commands
{
    public class CreateDailyCloseCommand : IRequest<Response<DailyCloseDto>>
    {
        public int BranchOfficeId { get; set; }
        public DateTime Date { get; set; }
        public int CaiId { get; set; }
    }
    public class CreateDailyCloseCommandHandler : IRequestHandler<CreateDailyCloseCommand, Response<DailyCloseDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<DailyClose> _repositoryAsync;
        private readonly IRepositoryAsync<BranchOffices> _branchOfficeRepositoryAsync;
        private readonly IOutputCacheStore _outputCacheStored;
        private readonly IRepositoryAsync<Cai> _caiRepositoryAsync;
        private readonly IRepositoryAsync<ResumePayment> _resumePaymentRepositoryAsync;
        private readonly IRepositoryAsync<Invoice> _invoiceRepositoryAsync;
        private readonly IJwtService _jwtService;
        public CreateDailyCloseCommandHandler(IMapper mapper, IRepositoryAsync<DailyClose> repositoryAsync, IRepositoryAsync<Invoice> invoiceRepositoryAsync, IRepositoryAsync<BranchOffices> branchOfficeRepositoryAsync, IOutputCacheStore outputCacheStore, IRepositoryAsync<Cai> caiRepositoryAsync, IRepositoryAsync<ResumePayment> resumePaymentRepositoryAsync, IJwtService jwtService)
        {
            _mapper = mapper;
            _repositoryAsync = repositoryAsync;
            _branchOfficeRepositoryAsync = branchOfficeRepositoryAsync;
            _outputCacheStored = outputCacheStore;
            _caiRepositoryAsync = caiRepositoryAsync;
            _resumePaymentRepositoryAsync = resumePaymentRepositoryAsync;
            _jwtService = jwtService;
            _invoiceRepositoryAsync = invoiceRepositoryAsync;
        }
        public async Task<Response<DailyCloseDto>> Handle(CreateDailyCloseCommand request, CancellationToken cancellationToken)
        {
            var branchOffice = await _branchOfficeRepositoryAsync.GetByIdAsync(request.BranchOfficeId);
            if (branchOffice == null)
            {
                throw new ApiException($"No existe una sucursal con el id {request.BranchOfficeId}");
            }
            var cai = await _caiRepositoryAsync.GetByIdAsync(request.CaiId);
            if (cai == null)
            {
                throw new ApiException($"No existe un CAI con el id {request.CaiId}");
            }
            var invoices = await _invoiceRepositoryAsync.ListAsync(new FilterInvoiceByCaiIdBranchOfficeIdAndDateSpecification(cai.Id, branchOffice.Id, DateOnly.FromDateTime(request.Date)));

            var dailyClose = new DailyClose
            {
                BranchOfficeId = request.BranchOfficeId,
                Date = DateOnly.FromDateTime(request.Date),
                CaiId = request.CaiId,
                Exonerated = invoices.Sum(x => x.Exonerated),
                Exempt = invoices.Sum(x => x.Exempt),
                TaxedAt15Percent = invoices.Sum(x => x.TaxedAt15Percent),
                TaxedAt18Percent = invoices.Sum(x => x.TaxedAt18Percent),
                Taxes15Percent = invoices.Sum(x => x.Taxes15Percent),
                Taxes18Percent = invoices.Sum(x => x.Taxes18Percent),
                Total = invoices.Sum(x => x.Total),
                NumberOfInvoices = invoices.Count(),
                StartCorrelative = invoices[0].InvoiceNumber,
                EndCorrelative = invoices[invoices.Count() - 1].InvoiceNumber,
                CreationDate = DateTime.Now,
                CreatedBy = _jwtService.GetSubjectToken()
            };
            var response = await _repositoryAsync.AddAsync(dailyClose);
            await _repositoryAsync.SaveChangesAsync();
            var resumePayments = await SaveResumePayments(invoices, response.Id);
            await _outputCacheStored.EvictByTagAsync("cache_dailyClosing", cancellationToken);
            response.Cai = cai;
            response.BranchOffice = branchOffice;
            response.ResumePayments = resumePayments;
            return new Response<DailyCloseDto>(_mapper.Map<DailyCloseDto>(response), "Cierre Guardado Exitosamente");
        }
        public async Task<List<ResumePayment>> SaveResumePayments(List<Invoice> invoices, int dailyCloseId)
        {
            var typeOfPaymentMethods = new List<TypeOfPaymentMethod>();
            var resumePayments = new List<ResumePayment>();
            foreach (var invoice in invoices)
            {
                if (invoice.BillPayments.Count() > 0)
                {
                    foreach (var billPayment in invoice.BillPayments)
                    {
                        if (!typeOfPaymentMethods.Any(x => x.Id == billPayment.TypeOfPaymentMethod.Id))
                        {
                            typeOfPaymentMethods.Add(billPayment.TypeOfPaymentMethod);
                        }
                    }
                }
            }
            foreach (var typeOfPaymentMethod in typeOfPaymentMethods)
            {
                var resumePayment = new ResumePayment
                {
                    DailyCloseId = dailyCloseId,
                    TypeOfPaymentMethodId = typeOfPaymentMethod.Id,

                    Amount = invoices.Where(x => x.BillPayments.Any(y => y.TypeOfPaymentMethod.Id == typeOfPaymentMethod.Id)).Sum(x => x.BillPayments.Where(y => y.TypeOfPaymentMethod.Id == typeOfPaymentMethod.Id).Sum(y => y.Amount))
                };
                resumePayments.Add(resumePayment);
            }
            foreach (var resumePayment in resumePayments)
            {
                await _resumePaymentRepositoryAsync.AddAsync(resumePayment);
                resumePayment.TypeOfPayment = typeOfPaymentMethods.FirstOrDefault(x => x.Id == resumePayment.TypeOfPaymentMethodId);
            }
            await _resumePaymentRepositoryAsync.SaveChangesAsync();
            return resumePayments;
        }
    }
}
