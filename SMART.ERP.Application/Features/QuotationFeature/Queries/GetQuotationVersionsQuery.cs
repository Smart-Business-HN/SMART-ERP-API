using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Quotation;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.QuotationSnapshotSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.QuotationFeature.Queries
{
    public class GetQuotationVersionsQuery : IRequest<PagedResponse<List<QuotationSnapshotDto>>>
    {
        public int QuotationId { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
    public class GetQuotationVersionsQueryHandler : IRequestHandler<GetQuotationVersionsQuery, PagedResponse<List<QuotationSnapshotDto>>>
    {
        private readonly IRepositoryAsync<QuotationSnapshot> _repositoryAsync;
        private readonly IMapper _mapper;
        public GetQuotationVersionsQueryHandler(IRepositoryAsync<QuotationSnapshot> repositoryAsync, IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _mapper = mapper;
        }
        public async Task<PagedResponse<List<QuotationSnapshotDto>>> Handle(GetQuotationVersionsQuery request, CancellationToken cancellationToken)
        {
            var snapshots = await _repositoryAsync.ListAsync(new GetQuotationSnapshotsByQuotationIdSpecification(request.QuotationId, request.PageNumber, request.PageSize));
            var totalItems = await _repositoryAsync.CountAsync(new CountSnapshotsByQuotationIdSpecification(request.QuotationId));
            var dto = _mapper.Map<List<QuotationSnapshotDto>>(snapshots);
            return new PagedResponse<List<QuotationSnapshotDto>>(dto, request.PageNumber, request.PageSize, totalItems);
        }
    }
}
