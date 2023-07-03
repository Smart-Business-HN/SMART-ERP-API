using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Opportunity;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.OpportunitySpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.OpportunityFeature.Queries
{
    public class GetOpportunityByIdQuery : IRequest<Response<OpportunityDto>>
    {
        public int Id { get; set; }
    }

    public class GetOpportunityByIdQueryHandler : IRequestHandler<GetOpportunityByIdQuery, Response<OpportunityDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<Opportunity> _repositoryAsync;
        private readonly IRepositoryAsync<Customer> _clientRepositoryAsync;
        public GetOpportunityByIdQueryHandler(IMapper mapper, IRepositoryAsync<Opportunity> repositoryAsync,
            IRepositoryAsync<Customer> clientRepositoryAsync)
        {
            _mapper = mapper;
            _repositoryAsync = repositoryAsync;
            _clientRepositoryAsync = clientRepositoryAsync;
        }
        public async Task<Response<OpportunityDto>> Handle(GetOpportunityByIdQuery request, CancellationToken cancellationToken)
        {
            var opportunity = await _repositoryAsync.FirstOrDefaultAsync(
                new OpportunityIncludesSpecification(id: request.Id, customerId: null));
            if (opportunity == null)
            {
                throw new KeyNotFoundException($"Registro no encontrado con el id {request.Id}");
            }
            opportunity.QuoteProducts = opportunity.QuoteProducts!.FindAll(x => x.IsActive);
            var dto = _mapper.Map<OpportunityDto>(opportunity);
            var client = await _clientRepositoryAsync.GetByIdAsync(dto.Customer!.MotorsId);
            if (client != null)
            {
                dto.Customer!.FullName = client.FullName;
                dto.Customer!.PhoneNumber = client.PhoneNumber;
                dto.Customer!.Email = client.Email;
            }
            return new Response<OpportunityDto>(dto);
        }
    }
}
