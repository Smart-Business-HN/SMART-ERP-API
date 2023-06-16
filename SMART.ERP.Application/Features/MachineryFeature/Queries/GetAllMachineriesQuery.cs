using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Rootcloud;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.MachinerySpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.MachineryFeature.Queries
{
    public class GetAllMachineriesQuery : IRequest<PagedResponse<List<MachineryNoListObjectsDto>>>
    {
        public string? Parameter { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? Order { get; set; }
        public string? Column { get; set; }
        public bool All { get; set; }
        public DateTime Date { get; set; }
    }

    public class GetAllMachineriesQueryHandler : IRequestHandler<GetAllMachineriesQuery, PagedResponse<List<MachineryNoListObjectsDto>>>
    {
        private readonly IRepositoryAsync<Machinery> _repositoryAsync;
        private readonly IMapper _mapper;

        public GetAllMachineriesQueryHandler(IRepositoryAsync<Machinery> repositoryAsync, IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _mapper = mapper;
        }
        public async Task<PagedResponse<List<MachineryNoListObjectsDto>>> Handle(GetAllMachineriesQuery request, CancellationToken cancellationToken)
        {
            if (request.All)
            {
                request.PageNumber = 0;
                request.PageSize = await _repositoryAsync.CountAsync();
            }
            List<MachineryNoListObjectsDto> result = new List<MachineryNoListObjectsDto>();
            var machineries = await _repositoryAsync.ListAsync(
                new PaginationMachinerySpecification(request.Parameter, request.PageNumber, request.PageSize, request.Order, request.Column));
            var dto = _mapper.Map<List<MachineryResumeDto>>(machineries);
            foreach (var item in dto)
            {
                var newObject = _mapper.Map<MachineryNoListObjectsDto>(item);
                newObject.MachineryFailureReport = item.MachineryFailureReports?.FirstOrDefault();
                newObject.MachineryMaintenance = item.MachineryMaintenances?.FirstOrDefault();
                newObject.MachineyRootcloudHistorical = item.MachineyRootcloudHistoricals?.FirstOrDefault();
                result.Add(newObject);
            }
            return new PagedResponse<List<MachineryNoListObjectsDto>>(result, request.PageNumber, request.PageSize, request.All ? request.PageSize : await _repositoryAsync.CountAsync());
        }
    }
}
