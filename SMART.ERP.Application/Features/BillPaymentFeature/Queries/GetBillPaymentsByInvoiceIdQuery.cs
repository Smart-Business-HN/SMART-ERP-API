using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.BillPayment;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.BillPaymentSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.BillPaymentFeature.Queries
{
    public class GetBillPaymentsByInvoiceIdQuery : IRequest<Response<List<BillPaymentDto>>>
    {
        public int InvoiceId { get; set; }
    }
    public class GetBillPaymentsByInvoiceIdQueryHandler : IRequestHandler<GetBillPaymentsByInvoiceIdQuery, Response<List<BillPaymentDto>>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<BillPayment> _repositoryAsync;
        private readonly IRepositoryAsync<Invoice> _invoiceRepositoryAsync;

        public GetBillPaymentsByInvoiceIdQueryHandler(IMapper mapper, IRepositoryAsync<BillPayment> repositoryAsync, IRepositoryAsync<Invoice> invoiceRepositoryAsync)
        {
            _mapper = mapper;
            _repositoryAsync = repositoryAsync;
            _invoiceRepositoryAsync = invoiceRepositoryAsync;
        }

        public async Task<Response<List<BillPaymentDto>>> Handle(GetBillPaymentsByInvoiceIdQuery request, CancellationToken cancellationToken)
        {
            var invoiceExist = await _invoiceRepositoryAsync.GetByIdAsync(request.InvoiceId);
            if (invoiceExist == null)
            {
                throw new KeyNotFoundException($"Factura no encontrada con el id {request.InvoiceId}");
            }

            var billPayments = await _repositoryAsync.ListAsync(new FilterBillPaymentByInvoiceIdSpecification(request.InvoiceId));

            var dto = _mapper.Map<List<BillPaymentDto>>(billPayments);
            return new Response<List<BillPaymentDto>>(dto);
        }
    }
}
