using AutoMapper;
using MediatR;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.OpportunitySpecification;
using SMART.ERP.Application.Specifications.UserSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Application.DTOs.Dashboard;
using SMART.ERP.Application.Specifications.InvoiceSpecification;

namespace SMART.ERP.Application.Features.DashboardFeature.Queries.AdvisorMetrics
{
    public class SalesFromYearQuery : IRequest<Response<List<SalesByAdvisorDto>>>
    {
        public int Year { get; set; }
        public int BranchOfficeId { get; set; }
    }

    public class SalesFromYearQueryHandler : IRequestHandler<SalesFromYearQuery, Response<List<SalesByAdvisorDto>>>
    {
        private readonly IRepositoryAsync<User> _repositoryAsync;
        private readonly IRepositoryAsync<Invoice> _invoiceRepositoryAsync;
        private readonly IMapper _mapper;

        public SalesFromYearQueryHandler(IRepositoryAsync<User> repositoryAsync, IRepositoryAsync<Invoice> invoiceRepositoryAsync, IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _invoiceRepositoryAsync = invoiceRepositoryAsync;
            _mapper = mapper;
        }

        public async Task<Response<List<SalesByAdvisorDto>>> Handle(SalesFromYearQuery request, CancellationToken cancellationToken)
        {

            string[] months = { "Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio", "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre" };
            var response = new List<SalesByAdvisorDto>();
            var salesAdvisors = await _repositoryAsync.ListAsync(new FilterUserByRoleSpecification("Sales Advisor", request.BranchOfficeId));

            foreach (var user in salesAdvisors)
            {
                var dto = _mapper.Map<SalesByAdvisorDto>(user);
                var sales = await _invoiceRepositoryAsync.ListAsync(new FilterInvoicesInYearByUserSpecification(request.Year, user.Id));
                foreach (var month in months)
                {
                    foreach (var sale in sales)
                    {
                        if (sale.CreationDate!.Month == Array.IndexOf(months, month) + 1)
                        {
                            dto.Data[sale.CreationDate.Month - 1] += sale.Total;
                        }
                    }
                }
                response.Add(dto);

            }
            return new Response<List<SalesByAdvisorDto>>(response);
        }
    }
}
