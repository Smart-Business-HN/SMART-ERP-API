using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Quotation;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using Ardalis.Specification;

namespace SMART.ERP.Application.Features.QuotationPreviewFeature.Queries
{
    public class GetQuotationItemObservationsByQuotationIdQuery : IRequest<Response<List<QuotationItemObservationDto>>>
    {
        public int QuotationId { get; set; }
    }

    public class FilterItemObservationsByQuotationIdSpecification : Specification<QuotationItemObservation>
    {
        public FilterItemObservationsByQuotationIdSpecification(int quotationId)
        {
            Query.Where(x => x.QuotationId == quotationId)
                 .OrderBy(x => x.ProductOfferedId)
                 .ThenBy(x => x.CreationDate)
                 .AsNoTracking();
        }
    }

    public class GetQuotationItemObservationsByQuotationIdQueryHandler : IRequestHandler<GetQuotationItemObservationsByQuotationIdQuery, Response<List<QuotationItemObservationDto>>>
    {
        private readonly IRepositoryAsync<QuotationItemObservation> _observationRepository;
        private readonly IMapper _mapper;

        public GetQuotationItemObservationsByQuotationIdQueryHandler(
            IRepositoryAsync<QuotationItemObservation> observationRepository,
            IMapper mapper)
        {
            _observationRepository = observationRepository;
            _mapper = mapper;
        }

        public async Task<Response<List<QuotationItemObservationDto>>> Handle(GetQuotationItemObservationsByQuotationIdQuery request, CancellationToken cancellationToken)
        {
            var observations = await _observationRepository.ListAsync(
                new FilterItemObservationsByQuotationIdSpecification(request.QuotationId));

            var dto = _mapper.Map<List<QuotationItemObservationDto>>(observations);
            return new Response<List<QuotationItemObservationDto>>(dto);
        }
    }
}
