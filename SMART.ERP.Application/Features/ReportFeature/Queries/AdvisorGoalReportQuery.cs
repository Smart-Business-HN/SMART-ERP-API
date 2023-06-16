using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Report;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.AdvisorGoalSpecification;
using SMART.ERP.Application.Specifications.OpportunitySpecification;
using SMART.ERP.Application.Specifications.UserSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.ReportFeature.Queries
{
    public class AdvisorGoalReportQuery : IRequest<PagedResponse<List<ReportAdvisorGoalDto>>>
    {
        public int Year { get; set; }
        public Guid? UserId { get; set; }
        public int? BranchOfficeId { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public bool All { get; set; }
    }

    public class AdvisorGoalReportQueryHandler : IRequestHandler<AdvisorGoalReportQuery, PagedResponse<List<ReportAdvisorGoalDto>>>
    {
        private readonly IRepositoryAsync<AdvisorGoal> _repositoryAsync;
        private readonly IRepositoryAsync<User> _userRepositoryAsync;
        private readonly IRepositoryAsync<Opportunity> _opportunityRepositoryAsync;
        private readonly IMapper _mapper;

        public AdvisorGoalReportQueryHandler(IRepositoryAsync<AdvisorGoal> repositoryAsync, IRepositoryAsync<User> userRepositoryAsync,
            IRepositoryAsync<Opportunity> opportunityRepositoryAsync, IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _userRepositoryAsync = userRepositoryAsync;
            _opportunityRepositoryAsync = opportunityRepositoryAsync;
            _mapper = mapper;
        }

        public async Task<PagedResponse<List<ReportAdvisorGoalDto>>> Handle(AdvisorGoalReportQuery request, CancellationToken cancellationToken)
        {
            string[] months = { "Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio", "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre" };
            if (request.UserId != null)
            {
                var userResponse = new List<ReportAdvisorGoalDto>();
                var salesAdvisor = await _userRepositoryAsync.GetByIdAsync((Guid)request.UserId);
                if (salesAdvisor == null)
                {
                    throw new KeyNotFoundException($"No se encontro el usuario con id {request.UserId}");
                }
                var userAdvisorGoals = await _repositoryAsync.ListAsync(new FilterAdvisorGoalByYearSpecification(request.Year, request.UserId));
                var dto = new ReportAdvisorGoalDto();
                dto.FullName = salesAdvisor.FullName;
                var sales = await _opportunityRepositoryAsync.ListAsync(new FilterClosedOpportunitiesInYearByUserSpecification(request.Year, salesAdvisor.Id));
                sales = sales.FindAll(x => x.OpportunityStep.Name == "Ganado");
                foreach (var month in months)
                {
                    var monthDto = new ReportAdvisorGoalMonthDto();
                    monthDto.Month = month;
                    var findAdvisorGoal = userAdvisorGoals.FirstOrDefault(x => x.UserId == salesAdvisor.Id && x.InitDate.Month == Array.IndexOf(months, month) + 1);
                    if (findAdvisorGoal != null)
                    {
                        monthDto.Goal = findAdvisorGoal.Goal;
                    }
                    foreach (var sale in sales)
                    {
                        if (sale.ClosingDate!.Value.Month == Array.IndexOf(months, month) + 1)
                        {
                            monthDto.Total += sale.Total;
                        }
                    }
                    dto.Months.Add(monthDto);
                }
                userResponse.Add(dto);
                if (request.All)
                {
                    request.PageNumber = 0;
                    request.PageSize = userResponse.Count;
                }
                var pagedResult = userResponse.Skip(request.PageNumber * request.PageSize).Take(request.PageSize).ToList();
                return new PagedResponse<List<ReportAdvisorGoalDto>>(pagedResult, request.PageNumber, request.PageSize, userResponse.Count);
            }
            if (request.BranchOfficeId != null)
            {
                var branchResponse = new List<ReportAdvisorGoalDto>();
                var branchSalesAdvisors = new List<User>();
                branchSalesAdvisors = await _userRepositoryAsync.ListAsync(new FilterUserByRoleSpecification("Sales Advisor", request.BranchOfficeId));
                var branchAdvisorGoals = await _repositoryAsync.ListAsync(new FilterAdvisorGoalByYearSpecification(request.Year, null));
                foreach (var user in branchSalesAdvisors)
                {
                    var dto = new ReportAdvisorGoalDto();
                    dto.FullName = user.FullName;
                    var sales = await _opportunityRepositoryAsync.ListAsync(new FilterClosedOpportunitiesInYearByUserSpecification(request.Year, user.Id));
                    sales = sales.FindAll(x => x.OpportunityStep.Name == "Ganado");
                    foreach (var month in months)
                    {
                        var monthDto = new ReportAdvisorGoalMonthDto();
                        monthDto.Month = month;
                        var findAdvisorGoal = branchAdvisorGoals.FirstOrDefault(x => x.UserId == user.Id && x.InitDate.Month == Array.IndexOf(months, month) + 1);
                        if (findAdvisorGoal != null)
                        {
                            monthDto.Goal = findAdvisorGoal.Goal;
                        }
                        foreach (var sale in sales)
                        {
                            if (sale.ClosingDate!.Value.Month == Array.IndexOf(months, month) + 1)
                            {
                                monthDto.Total += sale.Total;
                            }
                        }
                        dto.Months.Add(monthDto);
                    }
                    branchResponse.Add(dto);
                }
                if (request.All)
                {
                    request.PageNumber = 0;
                    request.PageSize = branchResponse.Count;
                }
                var pagedResult = branchResponse.Skip(request.PageNumber * request.PageSize).Take(request.PageSize).ToList();
                return new PagedResponse<List<ReportAdvisorGoalDto>>(pagedResult, request.PageNumber, request.PageSize, branchResponse.Count);
            }
            var response = new List<ReportAdvisorGoalDto>();
            var salesAdvisors = new List<User>();
            salesAdvisors = await _userRepositoryAsync.ListAsync(new FilterUserByRoleSpecification("Sales Advisor", null));
            var advisorGoals = await _repositoryAsync.ListAsync(new FilterAdvisorGoalByYearSpecification(request.Year, null));
            foreach (var user in salesAdvisors)
            {
                var dto = new ReportAdvisorGoalDto();
                dto.FullName = user.FullName;
                var sales = await _opportunityRepositoryAsync.ListAsync(new FilterClosedOpportunitiesInYearByUserSpecification(request.Year, user.Id));
                sales = sales.FindAll(x => x.OpportunityStep.Name == "Ganado");
                foreach (var month in months)
                {
                    var monthDto = new ReportAdvisorGoalMonthDto();
                    monthDto.Month = month;
                    var findAdvisorGoal = advisorGoals.FirstOrDefault(x => x.UserId == user.Id && x.InitDate.Month == Array.IndexOf(months, month) + 1);
                    if (findAdvisorGoal != null)
                    {
                        monthDto.Goal = findAdvisorGoal.Goal;
                    }
                    foreach (var sale in sales)
                    {
                        if (sale.ClosingDate!.Value.Month == Array.IndexOf(months, month) + 1)
                        {
                            monthDto.Total += sale.Total;
                        }
                    }
                    dto.Months.Add(monthDto);
                }
                response.Add(dto);
            }
            if (request.All)
            {
                request.PageNumber = 0;
                request.PageSize = response.Count;
            }
            var pagedResponse = response.Skip(request.PageNumber * request.PageSize).Take(request.PageSize).ToList();
            return new PagedResponse<List<ReportAdvisorGoalDto>>(pagedResponse, request.PageNumber, request.PageSize, response.Count);
        }
    }
}
