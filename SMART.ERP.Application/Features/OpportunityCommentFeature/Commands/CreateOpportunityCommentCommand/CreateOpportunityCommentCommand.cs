using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Opportunity;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.JwtService;
using SMART.ERP.Application.Specifications.OpportunityCommentSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.OpportunityCommentFeature.Commands.CreateOpportunityCommentCommand
{
    public class CreateOpportunityCommentCommand : IRequest<Response<OpportunityCommentDto>>
    {
        public string Message { get; set; } = null!;
        public Guid UserId { get; set; }
        public int OpportunityId { get; set; }
    }

    public class CreateOpportunityCommentCommandHandler : IRequestHandler<CreateOpportunityCommentCommand, Response<OpportunityCommentDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<OpportunityComment> _repositoryAsync;
        private readonly IRepositoryAsync<User> _userRepositoryAsync;
        private readonly IJwtService _jwtService;

        public CreateOpportunityCommentCommandHandler(
            IMapper mapper,
            IRepositoryAsync<OpportunityComment> repositoryAsync,
            IRepositoryAsync<User> userRepositoryAsync,
            IJwtService jwtService)
        {
            _mapper = mapper;
            _repositoryAsync = repositoryAsync;
            _userRepositoryAsync = userRepositoryAsync;
            _jwtService = jwtService;
        }
        public async Task<Response<OpportunityCommentDto>> Handle(CreateOpportunityCommentCommand request, CancellationToken cancellationToken)
        {
            var opportunityComment = await _repositoryAsync.ListAsync(
                new FilterOpportunityCommentSpecification(null, request.UserId, request.OpportunityId, DateTime.Now));
            if (opportunityComment.Count >= 10)
                throw new ApiException($"Límite de comentarios excedidos por día");

            var getUser = await _userRepositoryAsync.GetByIdAsync(request.UserId);
            if (getUser == null)
                throw new ApiException($"No se encontro el usuario con el id: {request.UserId}");

            var newRecord = _mapper.Map<OpportunityComment>(request);
            newRecord.CreatedBy = _jwtService.GetSubjectToken();
            newRecord.CreationDate = DateTime.Now;
            var data = await _repositoryAsync.AddAsync(newRecord);
            data.User = getUser;
            await _repositoryAsync.SaveChangesAsync();
            var dto = _mapper.Map<OpportunityCommentDto>(data);
            return new Response<OpportunityCommentDto>(dto, message: $"Actividad creada exitosamente");
        }
    }
}
