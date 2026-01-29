using MediatR;
using SMART.ERP.Application.DTOs.Dashboard;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.InvoiceSpecification;
using SMART.ERP.Application.Specifications.UserSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.DashboardFeature.Queries.AdvisorMetrics
{
    public class NumSalesFromYearQuery : IRequest<Response<List<NumSalesByAdvisorDto>>>
    {
        public int Year { get; set; }
        public int BranchOfficeId { get; set; }
    }
    public class NumSalesFromYearQueryHandler : IRequestHandler<NumSalesFromYearQuery, Response<List<NumSalesByAdvisorDto>>>
    {
        private readonly IRepositoryAsync<User> _repositoryAsync;
        private readonly IRepositoryAsync<Invoice> _invoicesRepositoryAsync;

        public NumSalesFromYearQueryHandler(IRepositoryAsync<User> repositoryAsync, IRepositoryAsync<Invoice> invoiceRepositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
            _invoicesRepositoryAsync = invoiceRepositoryAsync;
        }

        public async Task<Response<List<NumSalesByAdvisorDto>>> Handle(NumSalesFromYearQuery request, CancellationToken cancellationToken)
        {
            var response = new List<NumSalesByAdvisorDto>();
            var salesAdvisors = new List<User>();
            salesAdvisors = await _repositoryAsync.ListAsync(new FilterUserByRoleSpecification("Sales Advisor", request.BranchOfficeId));

            foreach (var user in salesAdvisors)
            {
                var dto = new NumSalesByAdvisorDto();
                dto.FullName = user.FullName;
                var sales = await _invoicesRepositoryAsync.ListAsync(new FilterInvoicesInYearByUserSpecification(request.Year, user.Id));
                dto.NumSales = sales.Count;
                dto.Total = sales.Sum(a => a.Total);
                response.Add(dto);
            }
            return new Response<List<NumSalesByAdvisorDto>>(response);
        }
    }
}
