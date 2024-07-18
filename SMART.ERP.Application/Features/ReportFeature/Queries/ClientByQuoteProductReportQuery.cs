using MediatR;
using SMART.ERP.Application.DTOs.Report;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.ReportSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.ReportFeature.Queries
{
    public class ClientByQuoteProductReportQuery : IRequest<PagedResponse<List<ClientByQuoteProductDto>>>
    {
        public int ProductId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public bool All { get; set; }
    }

    public class ClientByQuoteProductReportQueryHandler : IRequestHandler<ClientByQuoteProductReportQuery, PagedResponse<List<ClientByQuoteProductDto>>>
    {
        private readonly IRepositoryAsync<Quotation> _repositoryAsync;

        public ClientByQuoteProductReportQueryHandler(IRepositoryAsync<Quotation> repositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
        }

        public async Task<PagedResponse<List<ClientByQuoteProductDto>>> Handle(ClientByQuoteProductReportQuery request, CancellationToken cancellationToken)
        {
            var quotations = await _repositoryAsync.ListAsync(new ClientByQuoteProductReportSpecification(request.ProductId));

            List<string> customers = new();
            foreach (var invoice in quotations)
            {
                if (!customers.Exists(x => x == invoice.Customer.FullName))
                {
                    customers.Add(invoice.Customer.FullName);
                }
            }
            List<ClientByQuoteProductDto> response = new();
            foreach (var client in customers)
            {
                var invoicesForThisClient = quotations.FindAll(x => x.Customer.FullName == client);
                decimal totalPurchased = 0;
                decimal totalQuantity = 0;
                foreach (var invoice in invoicesForThisClient)
                {
                    if (invoice.ProductsOffered != null)
                    {
                        invoice.ProductsOffered!.ForEach(prd =>
                        {
                            if (prd.ProductId == request.ProductId)
                            {
                                totalQuantity += prd.Quantity;
                                totalPurchased += prd.TotalLine;
                            }
                        });
                    }
                }
                ClientByQuoteProductDto dto = new();
                dto.CustomerName = client;
                dto.Quantity = (int)totalQuantity;
                dto.Total = totalPurchased;
                response.Add(dto);
            }
            response.Sort(delegate (ClientByQuoteProductDto a, ClientByQuoteProductDto b)
            {
                return b.Quantity.CompareTo(a.Quantity);
            });
            if (request.All)
            {
                request.PageNumber = 0;
                request.PageSize = response.Count;
            }
            var pagedResult = response.Skip(request.PageNumber * request.PageSize).Take(request.PageSize).ToList();
            return new PagedResponse<List<ClientByQuoteProductDto>>(pagedResult, request.PageNumber, request.PageSize, response.Count);
        }
    }
}
