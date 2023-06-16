using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.SaleOrder;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.JwtService;
using SMART.ERP.Application.Specifications.FinancingPlanSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.FinancingPlanFeature.Commands.UpdateFinancingPlanCommand
{
    public class UpdateFinancingPlanCommand : IRequest<Response<FinancingPlanDto>>
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int ProviderId { get; set; }
        public bool IsActive { get; set; }
    }

    public class UpdateFinancingPlanCommandHandler : IRequestHandler<UpdateFinancingPlanCommand, Response<FinancingPlanDto>>
    {
        private readonly IRepositoryAsync<FinancingPlan> _repositoryAsync;
        private readonly IJwtService _jwtService;
        private readonly IMapper _mapper;

        public UpdateFinancingPlanCommandHandler(IMapper mapper, IRepositoryAsync<FinancingPlan> repositoryAsync,
            IJwtService jwtService)
        {
            _repositoryAsync = repositoryAsync;
            _jwtService = jwtService;
            _mapper = mapper;
        }
        public async Task<Response<FinancingPlanDto>> Handle(UpdateFinancingPlanCommand request, CancellationToken cancellationToken)
        {
            var financingPlan = await _repositoryAsync.GetByIdAsync(request.Id);
            if (financingPlan == null)
            {
                throw new KeyNotFoundException($"No se encontro ningun registro con el id {request.Id}");
            }
            var filterByName = await _repositoryAsync.FirstOrDefaultAsync(new FilterFinancingPlanSpecification(request.Name, request.Id));
            if (filterByName != null)
            {
                throw new ApiException($"Ya existe un registro con el nombre {request.Name}");
            }
            else
            {
                financingPlan.Name = request.Name;
                financingPlan.Description = request.Description;
                financingPlan.ProviderId = request.ProviderId;
                financingPlan.IsActive = request.IsActive;
                financingPlan.ModificatedBy = _jwtService.GetSubjectToken();
                financingPlan.ModificationDate = DateTime.Now;
                await _repositoryAsync.UpdateAsync(financingPlan);
                await _repositoryAsync.SaveChangesAsync();
                var dto = _mapper.Map<FinancingPlanDto>(financingPlan);
                return new Response<FinancingPlanDto>(dto, message: $"{financingPlan.Name} actualizado correctamente");
            }
        }
    }
}
