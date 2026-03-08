using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Quotation;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.JwtService;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.QuotationPreviewFeature.Commands.CreateQuotationAdminCommentCommand
{
    public class CreateQuotationAdminCommentCommand : IRequest<Response<QuotationCommentDto>>
    {
        public int QuotationId { get; set; }
        public string Message { get; set; } = null!;
        public Guid UserId { get; set; }
    }

    public class CreateQuotationAdminCommentCommandHandler : IRequestHandler<CreateQuotationAdminCommentCommand, Response<QuotationCommentDto>>
    {
        private readonly IRepositoryAsync<Quotation> _quotationRepository;
        private readonly IRepositoryAsync<QuotationComment> _commentRepository;
        private readonly IRepositoryAsync<User> _userRepository;
        private readonly IJwtService _jwtService;
        private readonly IMapper _mapper;

        public CreateQuotationAdminCommentCommandHandler(
            IRepositoryAsync<Quotation> quotationRepository,
            IRepositoryAsync<QuotationComment> commentRepository,
            IRepositoryAsync<User> userRepository,
            IJwtService jwtService,
            IMapper mapper)
        {
            _quotationRepository = quotationRepository;
            _commentRepository = commentRepository;
            _userRepository = userRepository;
            _jwtService = jwtService;
            _mapper = mapper;
        }

        public async Task<Response<QuotationCommentDto>> Handle(CreateQuotationAdminCommentCommand request, CancellationToken cancellationToken)
        {
            var quotation = await _quotationRepository.GetByIdAsync(request.QuotationId);
            if (quotation == null)
            {
                throw new ApiException($"No se encontró la cotización con el id {request.QuotationId}");
            }

            var user = await _userRepository.GetByIdAsync(request.UserId);
            if (user == null)
            {
                throw new ApiException($"No se encontró el usuario con el id {request.UserId}");
            }

            var newComment = new QuotationComment
            {
                QuotationId = request.QuotationId,
                AuthorName = user.FullName,
                Message = request.Message,
                IsFromClient = false,
                UserId = request.UserId,
                CreationDate = DateTime.Now,
                CreatedBy = _jwtService.GetSubjectToken()
            };

            var data = await _commentRepository.AddAsync(newComment);
            data.User = user;
            await _commentRepository.SaveChangesAsync();

            var dto = _mapper.Map<QuotationCommentDto>(data);
            dto.UserFullName = user.FullName;
            return new Response<QuotationCommentDto>(dto, "Respuesta agregada exitosamente.");
        }
    }
}
