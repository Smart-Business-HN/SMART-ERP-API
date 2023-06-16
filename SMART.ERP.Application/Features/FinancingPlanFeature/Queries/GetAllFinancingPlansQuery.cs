using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.SaleOrder;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.FinancingPlanSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.FinancingPlanFeature.Queries
{
    public class GetAllFinancingPlansQuery : IRequest<PagedResponse<List<FinancingPlanDto>>>
    {
        public string? Parameter { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? Order { get; set; }
        public string? Column { get; set; }
        public bool All { get; set; }
        public class GetAllDataSheetsQueryHandler : IRequestHandler<GetAllFinancingPlansQuery, PagedResponse<List<FinancingPlanDto>>>
        {
            private readonly IMapper _mapper;
            private readonly IRepositoryAsync<FinancingPlan> _repositoryAsync;

            public GetAllDataSheetsQueryHandler(IMapper mapper, IRepositoryAsync<FinancingPlan> repositoryAsync)
            {
                _mapper = mapper;
                _repositoryAsync = repositoryAsync;
            }
            public async Task<PagedResponse<List<FinancingPlanDto>>> Handle(GetAllFinancingPlansQuery request, CancellationToken cancellationToken)
            {
                if (request.All)
                {
                    request.PageNumber = 0;
                    request.PageSize = await _repositoryAsync.CountAsync();
                }

                var financingPlans = await _repositoryAsync.ListAsync(
                    new FilterAndPaginationFinancingPlanSpecification(request.Parameter, request.PageNumber, request.PageSize, request.Order, request.Column));
                var dto = _mapper.Map<List<FinancingPlanDto>>(financingPlans);
                return new PagedResponse<List<FinancingPlanDto>>(dto, request.PageNumber, request.PageSize, request.All ? request.PageSize : await _repositoryAsync.CountAsync());
            }
        }
    }
}
