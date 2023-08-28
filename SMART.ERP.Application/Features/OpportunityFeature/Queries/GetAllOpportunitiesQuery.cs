using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Opportunity;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.OpportunitySpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.OpportunityFeature.Queries
{
    public class GetAllOpportunitiesQuery : IRequest<PagedResponse<List<OpportunityDto>>>
    {
        public string? Parameter { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? Order { get; set; }
        public string? Column { get; set; }
        public bool All { get; set; }
        public class GetAllOpportunitiesQueryHandler : IRequestHandler<GetAllOpportunitiesQuery, PagedResponse<List<OpportunityDto>>>
        {
            private readonly IMapper _mapper;
            private readonly IRepositoryAsync<Opportunity> _repositoryAsync;
            private readonly IRepositoryAsync<Customer> _clientRepositoryAsync;
            public GetAllOpportunitiesQueryHandler(IMapper mapper, IRepositoryAsync<Opportunity> repositoryAsync,
                IRepositoryAsync<Customer> clientRepositoryAsync)
            {
                _mapper = mapper;
                _repositoryAsync = repositoryAsync;
                _clientRepositoryAsync = clientRepositoryAsync;
            }
            public async Task<PagedResponse<List<OpportunityDto>>> Handle(GetAllOpportunitiesQuery request, CancellationToken cancellationToken)
            {
                if (request.All)
                {
                    request.PageNumber = 0;
                    request.PageSize = await _repositoryAsync.CountAsync();
                }

                var opportunities = await _repositoryAsync.ListAsync(
                    new QueryOpportunitySpecification(request.Parameter, request.PageNumber, request.PageSize, request.Order, request.Column));

                var clients = await _clientRepositoryAsync.ListAsync();
                var dto = _mapper.Map<List<OpportunityDto>>(opportunities);
                for (int index = 0; index < dto.Count; index++)
                {
                    var client = clients.Find(x => x.Id == dto[index].Customer!.Id);
                    if (client != null)
                    {
                        dto[index].Customer!.FullName = client.FullName;
                        dto[index].Customer!.PhoneNumber = client.PhoneNumber;
                        dto[index].Customer!.Email = client.Email;
                    }
                }
                return new PagedResponse<List<OpportunityDto>>(dto, request.PageNumber, request.PageSize, request.All ? request.PageSize : await _repositoryAsync.CountAsync());
            }
        }
    }
}
