using MediatR;
using SMART.ERP.Application.DTOs.Report;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.OpportunitySpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.ReportFeature.Queries
{
    public class OpportunityOriginReportQuery : IRequest<PagedResponse<List<ReportOpportunityOriginDto>>>
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? BranchOfficeId { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public bool All { get; set; }
    }

    public class OpportunityOriginReportQueryHandler : IRequestHandler<OpportunityOriginReportQuery, PagedResponse<List<ReportOpportunityOriginDto>>>
    {
        private readonly IRepositoryAsync<TypeOrigin> _repositoryAsync;
        private readonly IRepositoryAsync<Opportunity> _opportunityRepositoryAsync;
        private readonly IRepositoryAsync<BranchOffices> _branchRepositoryAsync;

        public OpportunityOriginReportQueryHandler(IRepositoryAsync<TypeOrigin> repositoryAsync, IRepositoryAsync<Opportunity> opportunityRepositoryAsync,
            IRepositoryAsync<BranchOffices> branchRepositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
            _opportunityRepositoryAsync = opportunityRepositoryAsync;
            _branchRepositoryAsync = branchRepositoryAsync;
        }

        public async Task<PagedResponse<List<ReportOpportunityOriginDto>>> Handle(OpportunityOriginReportQuery request, CancellationToken cancellationToken)
        {
            if (request.BranchOfficeId != null && request.BranchOfficeId != 0)
            {
                var checkBranch = await _branchRepositoryAsync.GetByIdAsync((int)request.BranchOfficeId);
                if (checkBranch == null)
                {
                    throw new KeyNotFoundException($"No se encontro la sucursal con id {request.BranchOfficeId}");
                }
            }
            if (request.StartDate != null && request.EndDate != null)
            {
                if (request.StartDate > request.EndDate)
                {
                    throw new ApiException("La fecha inicial no debe ser mayor a la fecha final");
                }
            }
            var origins = await _repositoryAsync.ListAsync();
            var response = new List<ReportOpportunityOriginDto>();
            var opportunities = await _opportunityRepositoryAsync.ListAsync(new FilterOpportunitiesinDatesSpecification(request.StartDate, request.EndDate, request.BranchOfficeId));
            opportunities = opportunities.FindAll(x => x.TypeOriginId != null);
            foreach (var origin in origins)
            {
                var dto = new ReportOpportunityOriginDto();

                dto.Name = origin.Name;
                dto.TotalNum = opportunities.FindAll(x => x.TypeOriginId == origin.Id).Count;
                if (dto.TotalNum > 0)
                {
                    dto.Percentage = (decimal)dto.TotalNum / opportunities.Count * 100;
                }
                else
                {
                    dto.Percentage = 0;
                }
                response.Add(dto);
            }
            if (request.All)
            {
                request.PageNumber = 0;
                request.PageSize = response.Count;
            }
            var pagedResponse = response.Skip(request.PageNumber * request.PageSize).Take(request.PageSize).ToList();
            return new PagedResponse<List<ReportOpportunityOriginDto>>(pagedResponse, request.PageNumber, request.PageSize, response.Count);
        }
    }
}
