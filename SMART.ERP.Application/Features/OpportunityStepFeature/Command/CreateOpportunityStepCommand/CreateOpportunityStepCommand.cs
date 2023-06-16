using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Opportunity;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.JwtService;
using SMART.ERP.Application.Specifications.OpportunityStepSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.OpportunityStepFeature.Command.CreateOpportunityStepCommand
{
    public class CreateOpportunityStepCommand : IRequest<Response<OpportunityStepDto>>
    {
        public string Name { get; set; } = null!;
        public bool IsActive { get; set; }
    }

    public class CreateOpportunityStepCommandHandler : IRequestHandler<CreateOpportunityStepCommand, Response<OpportunityStepDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<OpportunityStep> _repositoryAsync;
        private readonly IJwtService _jwtService;

        public CreateOpportunityStepCommandHandler(IMapper mapper, IRepositoryAsync<OpportunityStep> repositoryAsync, IJwtService jwtService)
        {
            _mapper = mapper;
            _repositoryAsync = repositoryAsync;
            _jwtService = jwtService;
        }

        public async Task<Response<OpportunityStepDto>> Handle(CreateOpportunityStepCommand request, CancellationToken cancellationToken)
        {
            var checkIfExistByName = await _repositoryAsync.FirstOrDefaultAsync(
                new FilterOpportunityStepSpecification(request.Name, null));
            if (checkIfExistByName != null)
            {
                throw new ApiException($"Ya existe un registro con el nombre {request.Name}");
            }

            var newRecord = _mapper.Map<OpportunityStep>(request);
            newRecord.CreationDate = DateTime.Now;
            newRecord.CreatedBy = _jwtService.GetSubjectToken();
            var data = await _repositoryAsync.AddAsync(newRecord);
            await _repositoryAsync.SaveChangesAsync();
            var dto = _mapper.Map<OpportunityStepDto>(data);

            return new Response<OpportunityStepDto>(dto, message: $"{request.Name} creado exitosamente");
        }
    }
}
