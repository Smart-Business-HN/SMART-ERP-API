using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.InvoicePaymentType;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.InvoicePaymentTypeSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.InvoicePaymentTypeFeature.Queries
{
    public class GetAllInvoicePaymentTypeQuery : IRequest<PagedResponse<List<InvoicePaymentTypeDto>>>
    {
        public string? Parameter { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? Order { get; set; }
        public string? Column { get; set; }
        public bool All { get; set; }
    }
    public class GetAllInvoicePaymentTypeQueryHandler : IRequestHandler<GetAllInvoicePaymentTypeQuery, PagedResponse<List<InvoicePaymentTypeDto>>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<InvoicePaymentType> _repositoryAsync;
        public GetAllInvoicePaymentTypeQueryHandler(IMapper mapper, IRepositoryAsync<InvoicePaymentType> repositoryAsync)
        {
            _mapper = mapper;
            _repositoryAsync = repositoryAsync;
        }

        public async Task<PagedResponse<List<InvoicePaymentTypeDto>>> Handle(GetAllInvoicePaymentTypeQuery request, CancellationToken cancellationToken)
        {
            if (request.All)
            {
                request.PageNumber = 0;
                request.PageSize = await _repositoryAsync.CountAsync();
            }
            var paymentTypes = await _repositoryAsync.ListAsync(new FilterAndPaginationInvoicePaymentTypeSpecification(request.Parameter, request.PageNumber, request.PageSize, request.Order, request.Column));
            var dto = _mapper.Map<List<InvoicePaymentTypeDto>>(paymentTypes);
            return new PagedResponse<List<InvoicePaymentTypeDto>>(dto, request.PageNumber, request.PageSize, request.All ? request.PageSize : await _repositoryAsync.CountAsync());

        }
    }
}
