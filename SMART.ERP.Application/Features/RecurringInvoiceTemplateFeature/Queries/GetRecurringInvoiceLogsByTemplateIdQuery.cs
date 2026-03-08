using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.RecurringInvoiceTemplate;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.RecurringInvoiceTemplateSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.RecurringInvoiceTemplateFeature.Queries
{
    public class GetRecurringInvoiceLogsByTemplateIdQuery : IRequest<PagedResponse<List<RecurringInvoiceLogDto>>>
    {
        public int TemplateId { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }

    public class GetRecurringInvoiceLogsByTemplateIdQueryHandler : IRequestHandler<GetRecurringInvoiceLogsByTemplateIdQuery, PagedResponse<List<RecurringInvoiceLogDto>>>
    {
        private readonly IRepositoryAsync<RecurringInvoiceLog> _repositoryAsync;
        private readonly IMapper _mapper;

        public GetRecurringInvoiceLogsByTemplateIdQueryHandler(IRepositoryAsync<RecurringInvoiceLog> repositoryAsync, IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _mapper = mapper;
        }

        public async Task<PagedResponse<List<RecurringInvoiceLogDto>>> Handle(GetRecurringInvoiceLogsByTemplateIdQuery request, CancellationToken cancellationToken)
        {
            var logs = await _repositoryAsync.ListAsync(new FilterRecurringInvoiceLogsByTemplateIdSpecification(request.TemplateId, request.PageNumber, request.PageSize));
            var dto = _mapper.Map<List<RecurringInvoiceLogDto>>(logs);
            var totalCount = await _repositoryAsync.CountAsync();
            return new PagedResponse<List<RecurringInvoiceLogDto>>(dto, request.PageNumber, request.PageSize, totalCount);
        }
    }
}
