using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Quotation;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.QuotationCommentSpecification;
using SMART.ERP.Application.Specifications.QuotationSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.QuotationPreviewFeature.Commands.CreateQuotationClientCommentCommand
{
    public class CreateQuotationClientCommentCommand : IRequest<Response<QuotationCommentDto>>
    {
        public Guid AccessToken { get; set; }
        public string AuthorName { get; set; } = null!;
        public string? AuthorEmail { get; set; }
        public string Message { get; set; } = null!;
    }

    public class CreateQuotationClientCommentCommandHandler : IRequestHandler<CreateQuotationClientCommentCommand, Response<QuotationCommentDto>>
    {
        private readonly IRepositoryAsync<Quotation> _quotationRepository;
        private readonly IRepositoryAsync<QuotationComment> _commentRepository;
        private readonly IMapper _mapper;

        public CreateQuotationClientCommentCommandHandler(
            IRepositoryAsync<Quotation> quotationRepository,
            IRepositoryAsync<QuotationComment> commentRepository,
            IMapper mapper)
        {
            _quotationRepository = quotationRepository;
            _commentRepository = commentRepository;
            _mapper = mapper;
        }

        public async Task<Response<QuotationCommentDto>> Handle(CreateQuotationClientCommentCommand request, CancellationToken cancellationToken)
        {
            var quotation = await _quotationRepository.FirstOrDefaultAsync(
                new FilterQuotationByAccessTokenSpecification(request.AccessToken));

            if (quotation == null)
            {
                throw new ApiException("Cotización no encontrada o el enlace es inválido.");
            }

            // Check daily comment limit (20 per day per quotation)
            var todayComments = await _commentRepository.ListAsync(
                new FilterQuotationCommentSpecification(quotation.Id, DateTime.Now));

            if (todayComments.Count >= 20)
            {
                throw new ApiException("Se ha alcanzado el límite de comentarios por día para esta cotización.");
            }

            var newComment = new QuotationComment
            {
                QuotationId = quotation.Id,
                AuthorName = request.AuthorName,
                AuthorEmail = request.AuthorEmail,
                Message = request.Message,
                IsFromClient = true,
                CreationDate = DateTime.Now,
                CreatedBy = request.AuthorName
            };

            var data = await _commentRepository.AddAsync(newComment);
            await _commentRepository.SaveChangesAsync();

            var dto = _mapper.Map<QuotationCommentDto>(data);
            return new Response<QuotationCommentDto>(dto, "Comentario agregado exitosamente.");
        }
    }
}
