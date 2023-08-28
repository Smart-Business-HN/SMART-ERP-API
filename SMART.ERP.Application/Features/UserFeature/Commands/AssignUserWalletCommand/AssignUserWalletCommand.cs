using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Opportunity;
using SMART.ERP.Application.DTOs.User;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.ClientSpecification;
using SMART.ERP.Application.Specifications.CustomerSpecification;
using SMART.ERP.Application.Specifications.OpportunitySpecification;
using SMART.ERP.Application.Specifications.UserSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Application.DTOs.Customer;

namespace SMART.ERP.Application.Features.UserFeature.Commands.AssignUserWalletCommand
{
    public class AssignUserWalletCommand : IRequest<Response<UserWalletDto>>
    {
        public Guid CustomerId { get; set; }
        public Guid UserId { get; set; }
    }

    public class AssignUserWalletCommandHandler : IRequestHandler<AssignUserWalletCommand, Response<UserWalletDto>>
    {
        private readonly IRepositoryAsync<Customer> _repositoryAsync;
        private readonly IRepositoryAsync<User> _userRepositoryAsync;
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<Customer> _repositoryHNAsync;
        private readonly IRepositoryAsync<Opportunity> _opportunityRepositoryAsync;

        public AssignUserWalletCommandHandler(IRepositoryAsync<Customer> repositoryAsync, IRepositoryAsync<User> userRepositoryAsync, IMapper mapper,
            IRepositoryAsync<Customer> repositoryHNAsync, IRepositoryAsync<Opportunity> opportunityRepositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
            _userRepositoryAsync = userRepositoryAsync;
            _mapper = mapper;
            _repositoryHNAsync = repositoryHNAsync;
            _opportunityRepositoryAsync = opportunityRepositoryAsync;
        }

        public async Task<Response<UserWalletDto>> Handle(AssignUserWalletCommand request, CancellationToken cancellationToken)
        {
            var checkCustomer = await _repositoryAsync.GetByIdAsync(request.CustomerId);
            if (checkCustomer == null)
            {
                throw new KeyNotFoundException($"No se encontro el cliente con id {request.CustomerId}");
            }
            var checkUser = await _userRepositoryAsync.FirstOrDefaultAsync(new UserIncludesSpecification(request.UserId, null));
            if (checkUser == null)
            {
                throw new KeyNotFoundException($"No se encontro el usuario con id {request.UserId}");
            }
            if (checkUser!.Role!.Name != "Sales Advisor")
            {
                throw new ApiException("Solamente puedes asignar clientes a asesores de venta.");
            }
            if (checkCustomer!.UserId != null)
            {
                if (checkCustomer!.UserId == request.UserId)
                {
                    throw new ApiException("Este cliente ya se encuentra asignado a este usuario.");
                }
                var opportunities = await _opportunityRepositoryAsync.ListAsync(new FilterActiveOpportunitiesByCustomerSpecification(checkCustomer!.Id, false));
                foreach (var opportunity in opportunities)
                {
                    opportunity.UserId = request.UserId;
                }
                await _opportunityRepositoryAsync.UpdateRangeAsync(opportunities);
                await _opportunityRepositoryAsync.SaveChangesAsync();
            }
            checkCustomer!.UserId = request.UserId;
            await _repositoryAsync.UpdateAsync(checkCustomer);
            await _repositoryAsync.SaveChangesAsync();

            var userWallet = new UserWalletDto();
            userWallet.User = _mapper.Map<UserDto>(checkUser);
            userWallet.Clients = new List<ClientWalletDto>();
            userWallet.NumTemporalClients = 0;
            userWallet.NumOpportunity = 0;
            userWallet.NumClient = 0;
            var clients = await _repositoryHNAsync.ListAsync(new FilterClientByIdSpecification(null));
            var activeUserOpportunities = await _opportunityRepositoryAsync.ListAsync(new FilterOpportunityByUserSpecification(checkUser!.Id, true));
            var activeOpportunities = await _opportunityRepositoryAsync.ListAsync(new FilterActiveOpportunitiesByCustomerSpecification(null, true));
            var assignedClients = await _repositoryAsync.ListAsync(new FilterCustomerByUserSpecification(request.UserId));
            foreach (var customer in assignedClients)
            {
                var client = clients.FirstOrDefault(x => x.Id == customer.Id);
                var assignedClientActiveOpportunities = await _opportunityRepositoryAsync.ListAsync(new FilterActiveOpportunitiesByCustomerSpecification(customer.Id, true));
                var clientWallet = _mapper.Map<ClientWalletDto>(client);
                clientWallet.Assigned = true;
                clientWallet.NumOpportunities = assignedClientActiveOpportunities.Count;
                clientWallet.NumProducts = 0;
                clientWallet.UserId = customer.UserId;
                foreach (var opp in assignedClientActiveOpportunities)
                {
                    if (opp.QuoteProducts != null)
                    {
                        foreach (var quote in opp.QuoteProducts)
                        {
                            clientWallet.NumProducts += quote.Quantity;
                        }
                    }
                    clientWallet.Opportunities = _mapper.Map<List<OpportunityWalletDto>>(assignedClientActiveOpportunities);
                }
                userWallet.Clients.Add(clientWallet);
            }
            foreach (var opp in activeUserOpportunities)
            {
                if (userWallet.Clients.Exists(x => x.Id == opp.Customer!.Id))
                {
                    continue;
                }
                var client = clients.FirstOrDefault(x => x.Id == opp.Customer!.Id);
                if (client == null)
                {
                    throw new KeyNotFoundException($"No se encontro el cliente con id {opp.Customer!.Id}");
                }
                var clientWallet = _mapper.Map<ClientWalletDto>(client);
                var customerOpportunities = activeOpportunities.FindAll(x => x.CustomerId == opp.CustomerId && x.UserId == request.UserId);
                clientWallet.NumOpportunities = customerOpportunities.Count;
                clientWallet.NumProducts = 0;
                clientWallet.UserId = opp.Customer!.UserId;
                foreach (var opportunity in customerOpportunities)
                {
                    if (opportunity.QuoteProducts != null)
                    {
                        foreach (var quote in opportunity.QuoteProducts)
                        {
                            clientWallet.NumProducts += quote.Quantity;
                        }
                    }
                }
                clientWallet.Opportunities = _mapper.Map<List<OpportunityWalletDto>>(customerOpportunities);
                userWallet.Clients.Add(clientWallet);
            }
            userWallet.NumOpportunity = activeUserOpportunities.Count;
            userWallet.NumClient = userWallet.Clients.Count() - userWallet.NumTemporalClients;
            return new Response<UserWalletDto>(userWallet);
        }
    }
}
