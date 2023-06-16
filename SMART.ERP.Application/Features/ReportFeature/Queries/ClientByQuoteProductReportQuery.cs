using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Report;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.ClientSpecification;
using SMART.ERP.Application.Specifications.ReportSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.MASTER.Domain.Entities;
using SMART.ERP.Application.DTOs.Customer;

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
        private readonly IRepositoryAsync<Opportunity> _repositoryAsync;
        private readonly IRepositoryHNAsync<Client> _repositoryHNAsync;
        private readonly IMapper _mapper;

        public ClientByQuoteProductReportQueryHandler(IRepositoryAsync<Opportunity> repositoryAsync, IRepositoryHNAsync<Client> repositoryHNAsync,
            IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _repositoryHNAsync = repositoryHNAsync;
            _mapper = mapper;
        }

        public async Task<PagedResponse<List<ClientByQuoteProductDto>>> Handle(ClientByQuoteProductReportQuery request, CancellationToken cancellationToken)
        {
            var opportunities = await _repositoryAsync.ListAsync(new ClientByQuoteProductReportSpecification(request.ProductId));
            if (request.StartDate != null && request.EndDate != null)
            {
                var closed = opportunities.FindAll(x => (x.OpportunityStep.Name == "Ganado" || x.OpportunityStep.Name == "Perdido")
                    && x.ClosingDate.HasValue && x.ClosingDate.Value.Date >= request.StartDate.Value.Date
                    && x.ClosingDate.Value.Date <= request.EndDate.Value.Date);
                var notClosed = opportunities.FindAll(x => x.OpportunityStep.Name != "Ganado" && x.OpportunityStep.Name != "Perdido"
                    && x.CreationDate.Date >= request.StartDate.Value.Date && x.CreationDate.Date <= request.EndDate.Value.Date);
                opportunities = new HashSet<Opportunity>(closed.Concat(notClosed)).ToList();
            }
            List<Guid> guids = new List<Guid>();
            foreach (var opportunity in opportunities)
            {
                if (!guids.Exists(x => x == opportunity.Customer.MasterId))
                {
                    guids.Add(opportunity.Customer.MasterId);
                }
            }
            var clients = await _repositoryHNAsync.ListAsync(new FilterClientFromMotors(guids));
            List<ClientByQuoteProductDto> response = new();
            foreach (var client in clients)
            {
                var clientOpportunities = opportunities.FindAll(x => x.Customer.MasterId == client.Id);
                foreach (var opp in clientOpportunities)
                {
                    ClientByQuoteProductDto dto = new();
                    dto.Customer = _mapper.Map<CustomerDto>(client);
                    dto.Code = opp.Code;
                    dto.SalesAdvisor = opp.User!.FullName;
                    dto.OpportunityStep = opp.OpportunityStep!.Name;
                    response.Add(dto);
                }
            }
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
