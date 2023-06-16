using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Opportunity;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.JwtService;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.OpportunityCommentFeature.Commands.UpdateOpportunityCommentCommand
{
    public class UpdateOpportunityCommentCommand : IRequest<Response<OpportunityActivityDto>>
    {
        public int Id { get; set; }
        public string Message { get; set; } = null!;
        public Guid UserId { get; set; }
        public int OpportunityId { get; set; }
    }

    public class UpdateOpportunityCommentCommandHandler : IRequestHandler<UpdateOpportunityCommentCommand, Response<OpportunityActivityDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<OpportunityComment> _repositoryAsync;
        private readonly IJwtService _jwtService;

        public UpdateOpportunityCommentCommandHandler(IMapper mapper,
            IRepositoryAsync<OpportunityComment> repositoryAsync,
            IJwtService jwtService)
        {
            _mapper = mapper;
            _repositoryAsync = repositoryAsync;
            _jwtService = jwtService;
        }

        public async Task<Response<OpportunityActivityDto>> Handle(UpdateOpportunityCommentCommand request, CancellationToken cancellationToken)
        {
            var opportunityActivity = await _repositoryAsync.GetByIdAsync(request.Id);
            if (opportunityActivity == null)
            {
                throw new KeyNotFoundException($"No se encontro ningun registro con el id {request.Id}");
            }
            opportunityActivity.Message = request.Message;
            opportunityActivity.ModificationDate = DateTime.Now;
            opportunityActivity.ModificatedBy = _jwtService.GetSubjectToken();
            await _repositoryAsync.UpdateAsync(opportunityActivity);
            await _repositoryAsync.SaveChangesAsync();
            var dto = _mapper.Map<OpportunityActivityDto>(opportunityActivity);
            return new Response<OpportunityActivityDto>(dto, message: $"Actividad actualizada correctamente");
        }
    }
}
