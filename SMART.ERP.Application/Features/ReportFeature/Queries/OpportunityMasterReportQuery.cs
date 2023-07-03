using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Report;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.ClientSpecification;
using SMART.ERP.Application.Specifications.OpportunitySpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.ReportFeature.Queries
{
    public class OpportunityMasterReportQuery : IRequest<PagedResponse<List<ReportOpportunityMasterDto>>>
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? BranchOfficeId { get; set; }
        public Guid? UserId { get; set; }
        public int? OpportunityStepId { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public bool All { get; set; }
    }
    public class OpportunityMasterReportQueryHandler : IRequestHandler<OpportunityMasterReportQuery, PagedResponse<List<ReportOpportunityMasterDto>>>
    {
        private readonly IRepositoryAsync<Opportunity> _repositoryAsync;
        private readonly IRepositoryAsync<BranchOffices> _branchRepositoryAsync;
        private readonly IRepositoryAsync<User> _userRepositoryAsync;
        private readonly IRepositoryAsync<OpportunityStep> _stepRepositoryAsync;
        private readonly IRepositoryAsync<Customer> _customerRepositoryAsync;
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<Customer> _clientRepositoryAsync;

        public OpportunityMasterReportQueryHandler(IRepositoryAsync<Opportunity> repositoryAsync, IRepositoryAsync<BranchOffices> branchRepositoryAsync,
            IRepositoryAsync<User> userRepositoryAsync, IRepositoryAsync<OpportunityStep> stepRepositoryAsync,
            IRepositoryAsync<Customer> customerRepositoryAsync, IMapper mapper,
            IRepositoryAsync<Customer> clientRepositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
            _branchRepositoryAsync = branchRepositoryAsync;
            _userRepositoryAsync = userRepositoryAsync;
            _stepRepositoryAsync = stepRepositoryAsync;
            _customerRepositoryAsync = customerRepositoryAsync;
            _mapper = mapper;
            _clientRepositoryAsync = clientRepositoryAsync;
        }

        public async Task<PagedResponse<List<ReportOpportunityMasterDto>>> Handle(OpportunityMasterReportQuery request, CancellationToken cancellationToken)
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
            if (request.UserId != null)
            {
                var checkUser = await _userRepositoryAsync.GetByIdAsync((Guid)request.UserId);
                if (checkUser == null)
                {
                    throw new ApiException($"No se encontro el usuario con id {request.UserId}");
                }
            }
            var opportunities = await _repositoryAsync.ListAsync(new FilterOpportunitiesinDatesSpecification(request.StartDate, request.EndDate, request.BranchOfficeId));
            if (request.UserId != null)
            {
                opportunities = opportunities.FindAll(x => x.UserId == request.UserId);
            }
            if (request.OpportunityStepId != null)
            {
                var checkStep = await _stepRepositoryAsync.GetByIdAsync((int)request.OpportunityStepId);
                if (checkStep == null)
                {
                    throw new ApiException($"No se encontro el paso con id {request.OpportunityStepId}");
                }
                if (checkStep.Name == "Ganado" || checkStep.Name == "Perdido" || checkStep.Name == "Abandonado")
                {
                    if (request.StartDate.HasValue && request.EndDate.HasValue)
                    {
                        opportunities = opportunities.FindAll(x => x.OpportunityStepId == request.OpportunityStepId && (x.ClosingDate.HasValue &&
                        x.ClosingDate.Value.Date >= request.StartDate.Value.Date && x.ClosingDate.Value.Date <= request.EndDate.Value.Date ||
                        !x.ClosingDate.HasValue && x.CreationDate.Date >= request.StartDate.Value.Date && x.CreationDate.Date <= request.EndDate.Value.Date));
                    }
                    else
                    {
                        opportunities = opportunities.FindAll(x => x.OpportunityStepId == request.OpportunityStepId);
                    }
                }
                else
                {
                    opportunities = opportunities.FindAll(x => x.OpportunityStepId == request.OpportunityStepId);
                }

            }
            var clients = await _clientRepositoryAsync.ListAsync(new FilterClientByIdSpecification(null));
            var customers = await _customerRepositoryAsync.ListAsync();
            var response = new List<ReportOpportunityMasterDto>();
            foreach (var opportunity in opportunities)
            {
                var client = clients.Find(x => x.Id == customers.Find(y => y.Id == opportunity.CustomerId)!.Id);
                string ProductNames = "";
                for (int i = 0; i < opportunity.QuoteProducts!.Count; i++)
                {
                    if (i < opportunity.QuoteProducts!.Count - 1)
                    {
                        ProductNames += opportunity.QuoteProducts[i].Product!.Name + ", ";
                    }
                    else
                    {
                        ProductNames += opportunity.QuoteProducts[i].Product!.Name;
                    }

                }
                var dto = new ReportOpportunityMasterDto
                {
                    Code = opportunity.Code,
                    CreationDate = opportunity.CreationDate,
                    ClosingDate = opportunity.ClosingDate,
                    User = opportunity.User!.FullName,
                    Customer = client!.FullName,
                    Department = client!.DepartmentId != null ? client.Department!.Name : null,
                    Step = opportunity.OpportunityStep!.Name,
                    CustomerBudget = opportunity.Budget,
                    Products = ProductNames,
                    QtyItems = opportunity.QtyItems,
                    Total = opportunity.Total,
                    LossReason = opportunity.LossReasonId != null ? opportunity.LossReason!.Name : null,
                    WinReason = opportunity.WinReasonId != null ? opportunity.WinReason!.Name : null
                };
                response.Add(dto);
            }
            if (request.All)
            {
                request.PageNumber = 0;
                request.PageSize = response.Count;
            }
            var pagedResponse = response.Skip(request.PageNumber * request.PageSize).Take(request.PageSize).ToList();
            return new PagedResponse<List<ReportOpportunityMasterDto>>(pagedResponse, request.PageNumber, request.PageSize, response.Count);
        }
    }
}
