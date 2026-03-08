using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.RecurringInvoiceTemplate;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.RecurringInvoiceTemplateSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.RecurringInvoiceTemplateFeature.Queries
{
    public class GetAllRecurringInvoiceTemplateQuery : IRequest<PagedResponse<List<RecurringInvoiceTemplateDto>>>
    {
        public string? Parameter { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public bool All { get; set; }
    }

    public class GetAllRecurringInvoiceTemplateQueryHandler : IRequestHandler<GetAllRecurringInvoiceTemplateQuery, PagedResponse<List<RecurringInvoiceTemplateDto>>>
    {
        private readonly IRepositoryAsync<RecurringInvoiceTemplate> _repositoryAsync;
        private readonly IMapper _mapper;

        public GetAllRecurringInvoiceTemplateQueryHandler(IRepositoryAsync<RecurringInvoiceTemplate> repositoryAsync, IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _mapper = mapper;
        }

        public async Task<PagedResponse<List<RecurringInvoiceTemplateDto>>> Handle(GetAllRecurringInvoiceTemplateQuery request, CancellationToken cancellationToken)
        {
            if (request.All)
            {
                request.PageNumber = 0;
                request.PageSize = await _repositoryAsync.CountAsync();
            }
            var templates = await _repositoryAsync.ListAsync(new QueryRecurringInvoiceTemplateSpecification(request.Parameter, request.PageNumber, request.PageSize));
            var dto = _mapper.Map<List<RecurringInvoiceTemplateDto>>(templates);
            return new PagedResponse<List<RecurringInvoiceTemplateDto>>(dto, request.PageNumber, request.PageSize, request.All ? request.PageSize : await _repositoryAsync.CountAsync());
        }
    }
}
