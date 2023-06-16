using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.SaleOrder;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.FinancingPlanFeature.Queries
{
    public class GetFinancingPlanByIdQuery : IRequest<Response<FinancingPlanDto>>
    {
        public int Id { get; set; }
    }

    public class GetFinancingPlanByIdQueryHandler : IRequestHandler<GetFinancingPlanByIdQuery, Response<FinancingPlanDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<FinancingPlan> _repositoryAsync;

        public GetFinancingPlanByIdQueryHandler(IMapper mapper, IRepositoryAsync<FinancingPlan> repositoryAsync)
        {
            _mapper = mapper;
            _repositoryAsync = repositoryAsync;
        }
        public async Task<Response<FinancingPlanDto>> Handle(GetFinancingPlanByIdQuery request, CancellationToken cancellationToken)
        {
            var financingPlan = await _repositoryAsync.GetByIdAsync(request.Id);
            if (financingPlan == null)
            {
                throw new KeyNotFoundException($"Registro no encontrado con el id {request.Id}");
            }
            var dto = _mapper.Map<FinancingPlanDto>(financingPlan);
            return new Response<FinancingPlanDto>(dto);
        }
    }
}
