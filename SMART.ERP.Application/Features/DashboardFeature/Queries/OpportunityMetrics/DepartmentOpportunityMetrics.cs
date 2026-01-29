using MediatR;
using SMART.ERP.Application.DTOs.Dashboard;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.ClientSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.DashboardFeature.Queries.OpportunityMetrics
{
    public class DepartmentOpportunityMetrics : IRequest<Response<List<OpportunityDepartmentMetricDto>>>
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public class DepartmentOpportunityMetricsHandler : IRequestHandler<DepartmentOpportunityMetrics, Response<List<OpportunityDepartmentMetricDto>>>
    {
        private readonly IRepositoryAsync<Opportunity> _repositoryAsync;
        private readonly IRepositoryAsync<Customer> _clientRepositoryAsync;
        private readonly IRepositoryAsync<Customer> _customerRepositoryAsync;
        private readonly IRepositoryAsync<Department> _departmentRepositoryAsync;

        public DepartmentOpportunityMetricsHandler(IRepositoryAsync<Opportunity> repositoryAsync,
            IRepositoryAsync<Customer> clientRepositoryAsync,
            IRepositoryAsync<Customer> customerRepositoryAsync,
            IRepositoryAsync<Department> departmentRepositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
            _clientRepositoryAsync = clientRepositoryAsync;
            _customerRepositoryAsync = customerRepositoryAsync;
            _departmentRepositoryAsync = departmentRepositoryAsync;
        }
        public async Task<Response<List<OpportunityDepartmentMetricDto>>> Handle(DepartmentOpportunityMetrics request, CancellationToken cancellationToken)
        {
            var opportunities = await _repositoryAsync.ListAsync();
            var customers = await _customerRepositoryAsync.ListAsync();
            List<Guid> customersId = new List<Guid>();
            customers.ForEach(x =>
            {
                customersId.Add(x.Id);
            });
            var clients = await _clientRepositoryAsync.ListAsync(new FilterClientFromMotors(customersId));
            var departments = await _departmentRepositoryAsync.ListAsync();
            var response = new List<OpportunityDepartmentMetricDto>();
            foreach (var department in departments)
            {
                var clientsDepartment = clients.FindAll(x => x.DepartmentId == department.Id);
                if (clientsDepartment.Count > 0)
                {
                    var customerDepartment = customers.FindAll(x => clientsDepartment.Any(y => y.Id == x.Id));
                    if (customerDepartment.Count > 0)
                    {
                        var customerOpportunities = opportunities.FindAll(x => customerDepartment.Any(y => y.Id == x.CustomerId));
                        if (customerOpportunities.Count > 0)
                        {
                            OpportunityDepartmentMetricDto dto = new();
                            dto.Department = department.Name;
                            dto.NumOpportunities = customerOpportunities.Count;
                            dto.Total = customerOpportunities.Sum(x => x.Total);
                            response.Add(dto);
                        }
                    }
                }
            }
            OpportunityDepartmentMetricDto others = new();
            others.Department = "Otros";
            others.NumOpportunities = opportunities.FindAll(x => customers.Any(y => x.CustomerId == y.Id && clients.Any(z => y.Id == z.Id && z.DepartmentId == null))).Count;
            response.Add(others);
            return new Response<List<OpportunityDepartmentMetricDto>>(response);
        }
    }
}
