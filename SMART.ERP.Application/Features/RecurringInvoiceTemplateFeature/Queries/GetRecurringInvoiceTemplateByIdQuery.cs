using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.RecurringInvoiceTemplate;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.RecurringInvoiceTemplateSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.RecurringInvoiceTemplateFeature.Queries
{
    public class GetRecurringInvoiceTemplateByIdQuery : IRequest<Response<RecurringInvoiceTemplateDto>>
    {
        public int Id { get; set; }
    }

    public class GetRecurringInvoiceTemplateByIdQueryHandler : IRequestHandler<GetRecurringInvoiceTemplateByIdQuery, Response<RecurringInvoiceTemplateDto>>
    {
        private readonly IRepositoryAsync<RecurringInvoiceTemplate> _repositoryAsync;
        private readonly IMapper _mapper;

        public GetRecurringInvoiceTemplateByIdQueryHandler(IRepositoryAsync<RecurringInvoiceTemplate> repositoryAsync, IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _mapper = mapper;
        }

        public async Task<Response<RecurringInvoiceTemplateDto>> Handle(GetRecurringInvoiceTemplateByIdQuery request, CancellationToken cancellationToken)
        {
            var template = await _repositoryAsync.FirstOrDefaultAsync(new FilterRecurringInvoiceTemplateByIdSpecification(request.Id));
            if (template == null)
                throw new ApiException($"No existe una plantilla de factura recurrente con el Id {request.Id}");

            var dto = _mapper.Map<RecurringInvoiceTemplateDto>(template);
            return new Response<RecurringInvoiceTemplateDto>(dto);
        }
    }
}
