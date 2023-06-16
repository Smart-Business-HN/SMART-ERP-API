using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Opportunity;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.JwtService;
using SMART.ERP.Application.Specifications.OpportunityStepSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.OpportunityStepFeature.Command.UpdateOpportunityStepCommand
{
    public class UpdateOpportunityStepCommand : IRequest<Response<OpportunityStepDto>>
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public bool IsActive { get; set; }
    }

    public class UpdateOpportunityStepCommandHandler : IRequestHandler<UpdateOpportunityStepCommand, Response<OpportunityStepDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<OpportunityStep> _repositoryAsync;
        private readonly IJwtService _jwtService;

        public UpdateOpportunityStepCommandHandler(IMapper mapper, IRepositoryAsync<OpportunityStep> repositoryAsync, IJwtService jwtService)
        {
            _mapper = mapper;
            _repositoryAsync = repositoryAsync;
            _jwtService = jwtService;
        }

        public async Task<Response<OpportunityStepDto>> Handle(UpdateOpportunityStepCommand request, CancellationToken cancellationToken)
        {
            var opportunityStep = await _repositoryAsync.GetByIdAsync(request.Id);
            if (opportunityStep != null)
            {
                var checkIfExistByName = await _repositoryAsync.FirstOrDefaultAsync(
                new FilterOpportunityStepSpecification(request.Name, request.Id));
                if (checkIfExistByName != null)
                {
                    throw new ApiException($"Ya existe un registro con el nombre {request.Name}");
                }

                opportunityStep.Name = request.Name;
                opportunityStep.IsActive = request.IsActive;
                opportunityStep.ModificationDate = DateTime.Now;
                opportunityStep.ModificatedBy = _jwtService.GetSubjectToken();
                await _repositoryAsync.UpdateAsync(opportunityStep);
                await _repositoryAsync.SaveChangesAsync();
                var dto = _mapper.Map<OpportunityStepDto>(opportunityStep);
                return new Response<OpportunityStepDto>(dto, message: $"{opportunityStep.Name} actualizado correctamente");
            }
            else
            {
                throw new KeyNotFoundException($"No se encontro ningun registro con el id {request.Id}");
            }
        }
    }
}
