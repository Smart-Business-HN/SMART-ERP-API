using MediatR;
using SMART.ERP.Application.DTOs.AdvisorGoal;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.InvoiceSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.AdvisorGoalFeature.Queries
{
    public class GetAdvisorCommissionQuery : IRequest<Response<AdvisorCommissionDto>>
    {
        public Guid UserId { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
    }

    public class GetAdvisorCommissionQueryHandler : IRequestHandler<GetAdvisorCommissionQuery, Response<AdvisorCommissionDto>>
    {
        private readonly IRepositoryAsync<Invoice> _invoiceRepositoryAsync;
        private readonly IRepositoryAsync<User> _userRepositoryAsync;

        public GetAdvisorCommissionQueryHandler(IRepositoryAsync<Invoice> invoiceRepositoryAsync, IRepositoryAsync<User> userRepositoryAsync)
        {
            _invoiceRepositoryAsync = invoiceRepositoryAsync;
            _userRepositoryAsync = userRepositoryAsync;
        }

        public async Task<Response<AdvisorCommissionDto>> Handle(GetAdvisorCommissionQuery request, CancellationToken cancellationToken)
        {
            var user = await _userRepositoryAsync.GetByIdAsync(request.UserId);
            if (user == null)
            {
                throw new KeyNotFoundException($"No se encontro el usuario con id {request.UserId}");
            }

            var invoices = await _invoiceRepositoryAsync.ListAsync(
                new FilterInvoiceByMonthYearAndUserIdSpecification(request.Month, request.Year, request.UserId, null));

            decimal netSales = invoices.Sum(i => i.Total - i.Taxes15Percent - i.Taxes18Percent);

            var dto = new AdvisorCommissionDto
            {
                UserId = user.Id,
                FullName = user.FullName,
                Year = request.Year,
                Month = request.Month,
                NetSales = netSales,
                CommissionPercentage = user.CommissionPercentage,
                CommissionAmount = user.CommissionPercentage.HasValue
                    ? netSales * user.CommissionPercentage.Value / 100m
                    : (decimal?)null
            };

            return new Response<AdvisorCommissionDto>(dto);
        }
    }
}
