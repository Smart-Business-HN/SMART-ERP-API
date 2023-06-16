using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.SaleOrder;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.JwtService;
using SMART.ERP.Application.Specifications.FinancingPlanSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.FinancingPlanFeature.Commands.CreateFinancingPlanCommand
{
    public class CreateFinancingPlanCommand : IRequest<Response<FinancingPlanDto>>
    {
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int ProviderId { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateFinancingPlanCommandHandler : IRequestHandler<CreateFinancingPlanCommand, Response<FinancingPlanDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<FinancingPlan> _repositoryAsync;
        private readonly IJwtService _jwtService;

        public CreateFinancingPlanCommandHandler(IMapper mapper, IRepositoryAsync<FinancingPlan> repositoryAsync,
            IJwtService jwtService)
        {
            _mapper = mapper;
            _repositoryAsync = repositoryAsync;
            _jwtService = jwtService;
        }

        public async Task<Response<FinancingPlanDto>> Handle(CreateFinancingPlanCommand request, CancellationToken cancellationToken)
        {
            var checkIfExist = await _repositoryAsync.FirstOrDefaultAsync(new FilterFinancingPlanSpecification(request.Name, null));
            if (checkIfExist != null)
            {
                throw new ApiException($"Ya existe un registro con el nombre {request.Name}");
            }
            var newRecord = _mapper.Map<FinancingPlan>(request);
            newRecord.CreationDate = DateTime.Now;
            newRecord.CreatedBy = _jwtService.GetSubjectToken();
            var data = await _repositoryAsync.AddAsync(newRecord);
            await _repositoryAsync.SaveChangesAsync();
            var dto = _mapper.Map<FinancingPlanDto>(data);

            return new Response<FinancingPlanDto>(dto, message: $"{request.Name} creado exitosamente");
        }
    }
}
