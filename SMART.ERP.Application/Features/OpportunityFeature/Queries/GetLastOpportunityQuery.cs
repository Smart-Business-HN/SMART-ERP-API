using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Opportunity;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.CustomerSpecification;
using SMART.ERP.Application.Specifications.OpportunitySpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.MASTER.Domain.Entities;

namespace SMART.ERP.Application.Features.OpportunityFeature.Queries
{
    public class GetLastOpportunityQuery : IRequest<Response<ActiveOpportunityDto>>
    {
        public Guid CustomerId { get; set; }
    }

    public class GetLastOpportunityQueryHandler : IRequestHandler<GetLastOpportunityQuery, Response<ActiveOpportunityDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<Opportunity> _repositoryAsync;
        private readonly IRepositoryHNAsync<Client> _clientRepositoryAsync;
        private readonly IRepositoryAsync<Customer> _customerRepositoryAsync;

        public GetLastOpportunityQueryHandler(IMapper mapper, IRepositoryAsync<Opportunity> repositoryAsync,
            IRepositoryHNAsync<Client> clientRepositoryAsync, IRepositoryAsync<Customer> customerRepositoryAsync)
        {
            _mapper = mapper;
            _repositoryAsync = repositoryAsync;
            _clientRepositoryAsync = clientRepositoryAsync;
            _customerRepositoryAsync = customerRepositoryAsync;
        }
        public async Task<Response<ActiveOpportunityDto>> Handle(GetLastOpportunityQuery request, CancellationToken cancellationToken)
        {
            var customer = await _customerRepositoryAsync.FirstOrDefaultAsync(new FilterCustomerByMasterIdSpecification(request.CustomerId));
            if (customer == null)
            {
                throw new ApiException("Ocurrio un error al encontrar este cliente");
            }

            var opportunity = await _repositoryAsync.FirstOrDefaultAsync(
                new OpportunityIncludesSpecification(id: null, customerId: customer.Id));
            if (opportunity != null)
            {
                opportunity.QuoteProducts = opportunity.QuoteProducts!.FindAll(x => x.IsActive);
                var dto = _mapper.Map<OpportunityDto>(opportunity);
                var client = await _clientRepositoryAsync.GetByIdAsync(dto.Customer!.MasterId);
                if (client != null)
                {
                    dto.Customer!.FullName = client.FullName;
                    dto.Customer!.PhoneNumber = client.PhoneNumber;
                    dto.Customer!.Email = client.Email;
                }
                var result = new ActiveOpportunityDto()
                {
                    IsActive = true,
                    Opportunity = dto
                };
                return new Response<ActiveOpportunityDto>(result);
            }
            else
            {
                var result = new ActiveOpportunityDto()
                {
                    IsActive = false,
                };
                return new Response<ActiveOpportunityDto>(result);
            }
        }
    }
}
