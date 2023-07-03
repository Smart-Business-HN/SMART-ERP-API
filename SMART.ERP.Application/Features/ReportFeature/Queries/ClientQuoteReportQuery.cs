using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Product;
using SMART.ERP.Application.DTOs.Report;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.ClientSpecification;
using SMART.ERP.Application.Specifications.OpportunitySpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.ReportFeature.Queries
{
    public class ClientQuoteReportQuery : IRequest<PagedResponse<List<ReportClientQuoteDto>>>
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public bool All { get; set; }
    }

    public class ClientQuoteReportQueryHandler : IRequestHandler<ClientQuoteReportQuery, PagedResponse<List<ReportClientQuoteDto>>>
    {
        private readonly IRepositoryAsync<Customer> _repositoryAsync;
        private readonly IRepositoryAsync<Opportunity> _opportunityRepositoryAsync;
        private readonly IRepositoryAsync<Customer> _repositoryHNAsync;
        private readonly IMapper _mapper;

        public ClientQuoteReportQueryHandler(IRepositoryAsync<Customer> repositoryAsync, IRepositoryAsync<Opportunity> opportunityRepositoryAsync,
            IRepositoryAsync<Customer> repositoryHNAsync, IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _opportunityRepositoryAsync = opportunityRepositoryAsync;
            _repositoryHNAsync = repositoryHNAsync;
            _mapper = mapper;
        }

        public async Task<PagedResponse<List<ReportClientQuoteDto>>> Handle(ClientQuoteReportQuery request, CancellationToken cancellationToken)
        {
            var customers = await _repositoryAsync.ListAsync();
            var opportunities = await _opportunityRepositoryAsync.ListAsync(new OpportunityIncludesSpecification(null, null));
            if (request.StartDate != null && request.EndDate != null)
            {
                var closed = opportunities.FindAll(x => (x.OpportunityStep.Name == "Ganado" || x.OpportunityStep.Name == "Perdido"
                || x.OpportunityStep.Name == "Abandonado")
                    && x.ClosingDate.HasValue && x.ClosingDate.Value.Date >= request.StartDate.Value.Date
                    && x.ClosingDate.Value.Date <= request.EndDate.Value.Date);
                var notClosed = opportunities.FindAll(x => x.OpportunityStep.Name != "Ganado" && x.OpportunityStep.Name != "Perdido"
                    && x.OpportunityStep.Name != "Abandonado" && x.CreationDate.Date >= request.StartDate.Value.Date && x.CreationDate.Date <= request.EndDate.Value.Date);
                opportunities = new HashSet<Opportunity>(closed.Concat(notClosed)).ToList();
            }
            customers = customers.FindAll(x => opportunities.Exists(y => y.CustomerId == x.Id));
            List<Guid> guids = new();
            foreach (var customer in customers)
            {
                guids.Add(customer.Id);
            }
            var motorClients = await _repositoryHNAsync.ListAsync(new FilterClientFromMotors(guids));
            var response = new List<ReportClientQuoteDto>();
            foreach (var customer in motorClients)
            {
                ReportClientQuoteDto dto = new();
                dto.FullName = customer.FullName;
                Guid customerMotorId = customers.Find(x => x.Id == customer.Id)!.Id;
                var customerOpportunities = opportunities.FindAll(x => x.CustomerId == customerMotorId);
                foreach (var opportunity in customerOpportunities)
                {
                    if (opportunity.QuoteProducts != null)
                    {
                        foreach (var quote in opportunity.QuoteProducts)
                        {
                            if (!dto.Products.Exists(x => x.Product!.Name == quote.Product!.Name))
                            {
                                ClientQuoteProductDto productDto = new();
                                productDto.Product = _mapper.Map<BasicDetailProductDto>(quote.Product);
                                productDto.Quantity = quote.Quantity;
                                productDto.SalePrice = quote.SalePrice;
                                dto.Products.Add(productDto);
                            }
                            else
                            {
                                var getDto = dto.Products.Find(x => x.Product!.Name == quote.Product!.Name);
                                getDto!.Quantity += quote.Quantity;
                                getDto.SalePrice += quote.SalePrice;
                            }

                        }
                    }
                }
                if (dto.Products.Count > 0)
                {
                    response.Add(dto);
                }
            }
            if (request.All)
            {
                request.PageNumber = 0;
                request.PageSize = response.Count;
            }
            var pagedResult = response.Skip(request.PageNumber * request.PageSize).Take(request.PageSize).ToList();
            return new PagedResponse<List<ReportClientQuoteDto>>(pagedResult, request.PageNumber, request.PageSize, response.Count);
        }
    }
}
