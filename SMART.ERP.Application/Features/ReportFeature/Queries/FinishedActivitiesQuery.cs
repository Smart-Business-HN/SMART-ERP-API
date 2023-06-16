using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Opportunity;
using SMART.ERP.Application.DTOs.Report;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.ReportSpecification;
using SMART.ERP.Application.Specifications.UserSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.ReportFeature.Queries
{
    public class FinishedActivitiesQuery : IRequest<PagedResponse<List<ReportAdvisorActivitiesDto>>>
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? BranchOfficeId { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public bool All { get; set; }
    }

    public class FinishedActivitiesQueryHandler : IRequestHandler<FinishedActivitiesQuery, PagedResponse<List<ReportAdvisorActivitiesDto>>>
    {
        private readonly IRepositoryAsync<OpportunityActivity> _repositoryAsync;
        private readonly IRepositoryAsync<BranchOffices> _branchRepositoryAsync;
        private readonly IRepositoryAsync<User> _userRepositoryAsync;
        private readonly IMapper _mapper;

        public FinishedActivitiesQueryHandler(IRepositoryAsync<OpportunityActivity> repositoryAsync, IRepositoryAsync<BranchOffices> branchRepositoryAsync,
            IRepositoryAsync<User> userRepositoryAsync, IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _branchRepositoryAsync = branchRepositoryAsync;
            _userRepositoryAsync = userRepositoryAsync;
            _mapper = mapper;
        }

        public async Task<PagedResponse<List<ReportAdvisorActivitiesDto>>> Handle(FinishedActivitiesQuery request, CancellationToken cancellationToken)
        {
            if (request.StartDate != null && request.EndDate != null)
            {
                if (request.EndDate < request.StartDate)
                {
                    throw new ApiException("La fecha final debe ser mayor a la fecha inicial");
                }
            }
            if (request.BranchOfficeId != null)
            {
                var checkBranch = await _branchRepositoryAsync.GetByIdAsync((int)request.BranchOfficeId);
                if (checkBranch == null)
                {
                    throw new KeyNotFoundException($"No se encontro la sucursal con id {request.BranchOfficeId}");
                }
            }
            var advisors = await _userRepositoryAsync.ListAsync(new FilterUserByRoleSpecification("Sales Advisor", request.BranchOfficeId));
            var activities = await _repositoryAsync.ListAsync(new AdvisorActivitiesSpecification(request.StartDate, request.EndDate, request.BranchOfficeId));
            var response = new List<ReportAdvisorActivitiesDto>();
            advisors.ForEach(advisor =>
            {
                var dto = new ReportAdvisorActivitiesDto();
                dto.FullName = advisor.FullName;
                var userActivities = activities.FindAll(x => x.UserId == advisor.Id);
                if (userActivities.Count == 0)
                {
                    dto.Finished = 0;
                    dto.Pending = 0;
                    dto.Total = 0;
                    dto.CompletionPercentage = 0;
                    dto.Activities = new List<OpportunityActivityDto>();
                }
                else
                {
                    dto.Finished = userActivities.FindAll(x => x.Status!.Name == "Finalizado").Count;
                    dto.Pending = userActivities.FindAll(x => x.Status!.Name != "Finalizado").Count;
                    dto.Total = userActivities.Count;
                    decimal result;
                    decimal.TryParse(((decimal)dto.Finished / dto.Total * 100.0m).ToString("0.##"), out result);
                    dto.CompletionPercentage = result;
                    dto.Activities = _mapper.Map<List<OpportunityActivityDto>>(userActivities);
                }
                response.Add(dto);
            });
            if (request.All)
            {
                request.PageNumber = 0;
                request.PageSize = response.Count;
            }
            var pagedResponse = response.Skip(request.PageNumber * request.PageSize).Take(request.PageSize).ToList();
            return new PagedResponse<List<ReportAdvisorActivitiesDto>>(pagedResponse, request.PageNumber, request.PageSize, response.Count);
        }
    }
}
