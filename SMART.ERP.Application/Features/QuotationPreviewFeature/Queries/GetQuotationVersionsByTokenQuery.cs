using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Quotation;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.QuotationSnapshotSpecification;
using SMART.ERP.Application.Specifications.QuotationSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.QuotationPreviewFeature.Queries
{
    public class GetQuotationVersionsByTokenQuery : IRequest<Response<List<QuotationSnapshotDto>>>
    {
        public Guid AccessToken { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }

    public class GetQuotationVersionsByTokenQueryHandler : IRequestHandler<GetQuotationVersionsByTokenQuery, Response<List<QuotationSnapshotDto>>>
    {
        private readonly IRepositoryAsync<Quotation> _quotationRepository;
        private readonly IRepositoryAsync<QuotationSnapshot> _snapshotRepository;
        private readonly IMapper _mapper;

        public GetQuotationVersionsByTokenQueryHandler(
            IRepositoryAsync<Quotation> quotationRepository,
            IRepositoryAsync<QuotationSnapshot> snapshotRepository,
            IMapper mapper)
        {
            _quotationRepository = quotationRepository;
            _snapshotRepository = snapshotRepository;
            _mapper = mapper;
        }

        public async Task<Response<List<QuotationSnapshotDto>>> Handle(GetQuotationVersionsByTokenQuery request, CancellationToken cancellationToken)
        {
            var quotation = await _quotationRepository.FirstOrDefaultAsync(
                new FilterQuotationByAccessTokenSpecification(request.AccessToken));

            if (quotation == null)
            {
                throw new ApiException("Cotización no encontrada o el enlace es inválido.");
            }

            var snapshots = await _snapshotRepository.ListAsync(
                new GetQuotationSnapshotsByQuotationIdSpecification(quotation.Id, request.PageNumber, request.PageSize));

            var dto = _mapper.Map<List<QuotationSnapshotDto>>(snapshots);
            return new Response<List<QuotationSnapshotDto>>(dto);
        }
    }
}
