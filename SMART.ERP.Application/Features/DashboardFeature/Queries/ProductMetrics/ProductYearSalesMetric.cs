using MediatR;
using SMART.ERP.Application.DTOs.Dashboard;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.OpportunitySpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.DashboardFeature.Queries.ProductMetrics
{
    public class ProductYearSalesMetric : IRequest<Response<List<ProductYearSaleDto>>>
    {
        public int Year { get; set; }
        public int BranchOfficeId { get; set; }
    }

    public class ProductYearSalesMetricHandle : IRequestHandler<ProductYearSalesMetric, Response<List<ProductYearSaleDto>>>
    {
        private readonly IRepositoryAsync<Opportunity> _opportunityRepositoryAsync;

        public ProductYearSalesMetricHandle(IRepositoryAsync<Opportunity> opportunityRepositoryAsync)
        {
            _opportunityRepositoryAsync = opportunityRepositoryAsync;
        }

        public async Task<Response<List<ProductYearSaleDto>>> Handle(ProductYearSalesMetric request, CancellationToken cancellationToken)
        {
            string[] months = { "Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio", "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre" };
            var response = new List<ProductYearSaleDto>();
            var sales = new List<Opportunity>();
            sales = await _opportunityRepositoryAsync.ListAsync(new FilterClosedOpportunitiesInYearByBranchSpecification(request.Year, request.BranchOfficeId, true));
            foreach (var month in months)
            {

                var monthSale = sales.FindAll(x => x.ClosingDate!.Value.Month == Array.IndexOf(months, month) + 1);
                foreach (var sale in monthSale)
                {
                    if (sale.QuoteProducts != null)
                    {
                        foreach (var product in sale.QuoteProducts)
                        {
                            var index = response.FindIndex(x => x.Name == product.Product!.Name);
                            if (index != -1)
                            {
                                response[index].Data[Array.IndexOf(months, month)] = product.Quantity;
                            }
                            else
                            {
                                var dto = new ProductYearSaleDto();
                                dto.Name = product.Product!.Name;
                                dto.Data[Array.IndexOf(months, month)] = product.Quantity;
                                response.Add(dto);
                            }
                        }

                    }
                }
            }
            return new Response<List<ProductYearSaleDto>>(response);
        }
    }
}
