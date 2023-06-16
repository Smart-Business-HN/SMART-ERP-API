using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Rootcloud;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.MachinerySpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.MachineryFeature.Queries
{
    public class GetAllFailureReportsQuery : IRequest<PagedResponse<List<MachineryFailureReportDto>>>
    {
        public string? Parameter { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? Order { get; set; }
        public string? Column { get; set; }
        public bool All { get; set; }
    }

    public class GetAllFailureReportsQueryHandler : IRequestHandler<GetAllFailureReportsQuery, PagedResponse<List<MachineryFailureReportDto>>>
    {
        private readonly IRepositoryAsync<MachineryFailureReport> _repositoryAsync;
        private readonly IMapper _mapper;

        public GetAllFailureReportsQueryHandler(IRepositoryAsync<MachineryFailureReport> repositoryAsync,
            IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _mapper = mapper;
        }

        public async Task<PagedResponse<List<MachineryFailureReportDto>>> Handle(GetAllFailureReportsQuery request, CancellationToken cancellationToken)
        {
            if (request.All)
            {
                request.PageNumber = 0;
                request.PageSize = await _repositoryAsync.CountAsync();
            }

            var reports = await _repositoryAsync.ListAsync(
                new PaginationFailureReportSpecification(request.Parameter, request.PageNumber, request.PageSize, request.Order, request.Column));
            var dto = _mapper.Map<List<MachineryFailureReportDto>>(reports);

            return new PagedResponse<List<MachineryFailureReportDto>>(dto, request.PageNumber, request.PageSize, request.All ? request.PageSize : await _repositoryAsync.CountAsync());
        }
    }
}
