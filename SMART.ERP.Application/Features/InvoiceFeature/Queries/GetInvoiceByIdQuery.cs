using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Invoice;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.InvoiceSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.InvoiceFeature.Queries
{
    public class GetInvoiceByIdQuery :IRequest<Response<InvoiceDto>>
    {
        public int Id { get; set; }
    }
    public class GetInvoiceByIdQueryHandler : IRequestHandler<GetInvoiceByIdQuery, Response<InvoiceDto>>
    {
        private readonly IRepositoryAsync<Invoice> _repositoryAsync;
        private readonly IMapper _mapper;
        public GetInvoiceByIdQueryHandler(IRepositoryAsync<Invoice> repositoryAsync, IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _mapper = mapper;
        }
        public async Task<Response<InvoiceDto>> Handle(GetInvoiceByIdQuery request, CancellationToken cancellationToken)
        {
            var getQuotation = await _repositoryAsync.FirstOrDefaultAsync(new FilterInvoiceByIdSpecification(request.Id));
            if (getQuotation == null)
            {
                throw new KeyNotFoundException($"Registro no encontrado con el id {request.Id}");
            }
            var dto = _mapper.Map<InvoiceDto>(getQuotation);
            return new Response<InvoiceDto>(dto);

        }
    }
}
