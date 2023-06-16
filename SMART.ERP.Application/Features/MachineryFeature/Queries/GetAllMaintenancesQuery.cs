using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Rootcloud;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.MachinerySpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.MachineryFeature.Queries
{
    public class GetAllMaintenancesQuery : IRequest<PagedResponse<List<MachineryMaintenanceDto>>>
    {
        public string? Parameter { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? Order { get; set; }
        public string? Column { get; set; }
        public bool All { get; set; }
    }

    public class GetAllMaintenancesQueryHandler : IRequestHandler<GetAllMaintenancesQuery, PagedResponse<List<MachineryMaintenanceDto>>>
    {
        private readonly IRepositoryAsync<MachineryMaintenance> _repositoryAsync;
        private readonly IMapper _mapper;

        public GetAllMaintenancesQueryHandler(IRepositoryAsync<MachineryMaintenance> repositoryAsync, IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _mapper = mapper;
        }
        public async Task<PagedResponse<List<MachineryMaintenanceDto>>> Handle(GetAllMaintenancesQuery request, CancellationToken cancellationToken)
        {
            if (request.All)
            {
                request.PageNumber = 0;
                request.PageSize = await _repositoryAsync.CountAsync();
            }

            var maintenances = await _repositoryAsync.ListAsync(new PaginationMaintenanceSpecification(request.Parameter, request.PageNumber, request.PageSize, request.Order, request.Column));
            var dto = _mapper.Map<List<MachineryMaintenanceDto>>(maintenances);
            return new PagedResponse<List<MachineryMaintenanceDto>>(dto, request.PageNumber, request.PageSize, request.All ? request.PageSize : await _repositoryAsync.CountAsync());
        }
    }
}
