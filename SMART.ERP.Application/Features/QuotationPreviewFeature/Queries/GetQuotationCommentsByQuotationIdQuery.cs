using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Quotation;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.QuotationCommentSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.QuotationPreviewFeature.Queries
{
    public class GetQuotationCommentsByQuotationIdQuery : IRequest<Response<List<QuotationCommentDto>>>
    {
        public int QuotationId { get; set; }
    }

    public class GetQuotationCommentsByQuotationIdQueryHandler : IRequestHandler<GetQuotationCommentsByQuotationIdQuery, Response<List<QuotationCommentDto>>>
    {
        private readonly IRepositoryAsync<QuotationComment> _commentRepository;
        private readonly IMapper _mapper;

        public GetQuotationCommentsByQuotationIdQueryHandler(
            IRepositoryAsync<QuotationComment> commentRepository,
            IMapper mapper)
        {
            _commentRepository = commentRepository;
            _mapper = mapper;
        }

        public async Task<Response<List<QuotationCommentDto>>> Handle(GetQuotationCommentsByQuotationIdQuery request, CancellationToken cancellationToken)
        {
            var comments = await _commentRepository.ListAsync(
                new FilterQuotationCommentSpecification(request.QuotationId));

            var dto = _mapper.Map<List<QuotationCommentDto>>(comments);
            return new Response<List<QuotationCommentDto>>(dto);
        }
    }
}
