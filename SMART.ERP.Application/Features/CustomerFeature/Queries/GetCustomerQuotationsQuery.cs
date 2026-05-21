using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Customer;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.QuotationSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.CustomerFeature.Queries
{
    public class GetCustomerQuotationsQuery : IRequest<PagedResponse<List<CustomerQuotationLineDto>>>
    {
        public Guid CustomerId { get; set; }
        public string? Parameter { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }

    public class GetCustomerQuotationsQueryHandler : IRequestHandler<GetCustomerQuotationsQuery, PagedResponse<List<CustomerQuotationLineDto>>>
    {
        private readonly IRepositoryAsync<Quotation> _repositoryAsync;
        private readonly IMapper _mapper;

        public GetCustomerQuotationsQueryHandler(IRepositoryAsync<Quotation> repositoryAsync, IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _mapper = mapper;
        }

        public async Task<PagedResponse<List<CustomerQuotationLineDto>>> Handle(GetCustomerQuotationsQuery request, CancellationToken cancellationToken)
        {
            var pageNumber = request.PageNumber < 0 ? 0 : request.PageNumber;
            var pageSize = request.PageSize <= 0 ? 10 : request.PageSize;

            var quotations = await _repositoryAsync.ListAsync(
                new FilterQuotationByCustomerIdSpecification(request.CustomerId, request.Parameter, pageNumber, pageSize),
                cancellationToken);

            var totalItems = await _repositoryAsync.CountAsync(
                new CountQuotationByCustomerIdSpecification(request.CustomerId, request.Parameter),
                cancellationToken);

            var dto = _mapper.Map<List<CustomerQuotationLineDto>>(quotations);
            return new PagedResponse<List<CustomerQuotationLineDto>>(dto, pageNumber, pageSize, totalItems);
        }
    }
}
