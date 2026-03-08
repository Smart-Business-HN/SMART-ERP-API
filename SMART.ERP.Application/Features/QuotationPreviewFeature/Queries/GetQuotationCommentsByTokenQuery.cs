using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Quotation;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.QuotationCommentSpecification;
using SMART.ERP.Application.Specifications.QuotationSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.QuotationPreviewFeature.Queries
{
    public class GetQuotationCommentsByTokenQuery : IRequest<Response<List<QuotationCommentDto>>>
    {
        public Guid AccessToken { get; set; }
    }

    public class GetQuotationCommentsByTokenQueryHandler : IRequestHandler<GetQuotationCommentsByTokenQuery, Response<List<QuotationCommentDto>>>
    {
        private readonly IRepositoryAsync<Quotation> _quotationRepository;
        private readonly IRepositoryAsync<QuotationComment> _commentRepository;
        private readonly IMapper _mapper;

        public GetQuotationCommentsByTokenQueryHandler(
            IRepositoryAsync<Quotation> quotationRepository,
            IRepositoryAsync<QuotationComment> commentRepository,
            IMapper mapper)
        {
            _quotationRepository = quotationRepository;
            _commentRepository = commentRepository;
            _mapper = mapper;
        }

        public async Task<Response<List<QuotationCommentDto>>> Handle(GetQuotationCommentsByTokenQuery request, CancellationToken cancellationToken)
        {
            var quotation = await _quotationRepository.FirstOrDefaultAsync(
                new FilterQuotationByAccessTokenSpecification(request.AccessToken));

            if (quotation == null)
            {
                throw new ApiException("Cotización no encontrada o el enlace es inválido.");
            }

            var comments = await _commentRepository.ListAsync(
                new FilterQuotationCommentSpecification(quotation.Id));

            var dto = comments.Select(c =>
            {
                var commentDto = _mapper.Map<QuotationCommentDto>(c);
                if (!c.IsFromClient && c.User != null)
                {
                    commentDto.UserFullName = c.User.FullName;
                }
                return commentDto;
            }).ToList();

            return new Response<List<QuotationCommentDto>>(dto);
        }
    }
}
