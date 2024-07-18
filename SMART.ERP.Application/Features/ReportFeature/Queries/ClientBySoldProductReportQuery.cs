using MediatR;
using SMART.ERP.Application.DTOs.Report;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.ReportSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.ReportFeature.Queries
{
    public class ClientBySoldProductReportQuery : IRequest<PagedResponse<List<ClientByProductSoldDto>>>
    {
        public int ProductId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public bool All { get; set; }
    }

    public class ClientBySoldProductReportQueryHandler : IRequestHandler<ClientBySoldProductReportQuery, PagedResponse<List<ClientByProductSoldDto>>>
    {
        private readonly IRepositoryAsync<Invoice> _repositoryAsync;

        public ClientBySoldProductReportQueryHandler(IRepositoryAsync<Invoice> repositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
        }

        public async Task<PagedResponse<List<ClientByProductSoldDto>>> Handle(ClientBySoldProductReportQuery request, CancellationToken cancellationToken)
        {
            var invoices = await _repositoryAsync.ListAsync(new ClientBySoldProductSpecification(request.ProductId));
            List<string> customers = new();
            foreach (var invoice in invoices)
            {
                if (!customers.Exists(x => x == invoice.Customer.FullName))
                {
                    customers.Add(invoice.Customer.FullName);
                }
            }
            List<ClientByProductSoldDto> response = new();
            foreach (var client in customers)
            {
                var invoicesForThisClient = invoices.FindAll(x => x.Customer.FullName == client);
                decimal totalPurchased = 0;
                decimal totalQuantity = 0;
                foreach (var invoice in invoicesForThisClient)
                {
                    if (invoice.ProductsSold != null)
                    {
                        invoice.ProductsSold!.ForEach(prd =>
                        {
                            if (prd.ProductId == request.ProductId)
                            {
                                totalQuantity += prd.Quantity;
                                totalPurchased += prd.TotalLine;
                            }
                        });
                    }
                }
                ClientByProductSoldDto dto = new();
                dto.CustomerName = client;
                dto.Quantity = (int)totalQuantity;
                dto.Total = totalPurchased;
                response.Add(dto);
            }
            response.Sort(delegate (ClientByProductSoldDto a, ClientByProductSoldDto b)
            {
                return b.Quantity.CompareTo(a.Quantity);
            });
            if (request.All)
            {
                request.PageNumber = 0;
                request.PageSize = response.Count;
            }
            var pagedResult = response.Skip(request.PageNumber * request.PageSize).Take(request.PageSize).ToList();
            return new PagedResponse<List<ClientByProductSoldDto>>(pagedResult, request.PageNumber, request.PageSize, response.Count);
        }
    }
}
